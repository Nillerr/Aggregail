using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Aggregail.MongoDB
{
    public sealed class MongoEventStore : IEventStore
    {
        private static int _isInitialized;

        private readonly IMongoCollection<RecordedEvent> _events;
        private readonly IEventSerializer _serializer;

        public MongoEventStore(IMongoDatabase database, string collection, IEventSerializer serializer)
        {
            _events = database.GetCollection<RecordedEvent>(collection);
            _serializer = serializer;
        }

        public async Task AppendToStreamAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long expectedVersion,
            IEnumerable<IPendingEvent> pendingEvents
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            await InitializeIndexesAsync();

            var stream = configuration.Name.Stream(id);

            var latestEvent = await _events
                .Find(e => e.Stream == stream)
                .SortByDescending(e => e.EventNumber)
                .FirstOrDefaultAsync();

            if (expectedVersion == ExpectedVersion.NoStream && latestEvent != null)
            {
                throw new WrongExpectedVersionException("", expectedVersion, latestEvent.EventNumber);
            }

            if (expectedVersion > ExpectedVersion.NoStream && latestEvent == null)
            {
                throw new WrongExpectedVersionException("", expectedVersion, null);
            }

            var currentVersion = latestEvent?.EventNumber ?? -1L;

            var recordedEvents = pendingEvents
                .Select((e, i) => new RecordedEvent
                    {
                        Stream = stream,
                        EventId = e.Id,
                        EventType = e.Type,
                        EventNumber = currentVersion + i + 1,
                        Data = e.Data(_serializer)
                    }
                )
                .ToArray();

            await _events.InsertManyAsync(recordedEvents);
        }

        public async Task<TAggregate?> AggregateAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            await InitializeIndexesAsync();

            var stream = configuration.Name.Stream(id);

            var createdRecordedEvent = await _events
                .Find(e => e.Stream == stream)
                .SortBy(e => e.EventNumber)
                .FirstOrDefaultAsync();

            if (createdRecordedEvent == null)
            {
                return null;
            }

            if (!configuration.Constructors.TryGetValue(createdRecordedEvent.EventType, out var constructor))
            {
                throw new InvalidOperationException(
                    $"Unrecognized construction event type: {createdRecordedEvent.EventType}"
                );
            }

            var aggregate = constructor(id, _serializer, createdRecordedEvent.Data);
            aggregate.Record(new RecordableEvent(createdRecordedEvent.EventNumber));

            var applicators = configuration.Applicators;

            var cursor = await _events
                .Find(e => e.Stream == stream)
                .SortBy(e => e.EventNumber)
                .Skip(1)
                .ToCursorAsync();

            while (await cursor.MoveNextAsync())
            {
                foreach (var recordedEvent in cursor.Current)
                {
                    if (applicators.TryGetValue(recordedEvent.EventType, out var applicator))
                    {
                        applicator(aggregate, _serializer, recordedEvent.Data);
                        aggregate.Record(new RecordableEvent(recordedEvent.EventNumber));
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unexpected recorded event type: {recordedEvent.EventType}"
                        );
                    }
                }
            }

            return aggregate;
        }

        private async Task InitializeIndexesAsync()
        {
            var isInitialized = Interlocked.Exchange(ref _isInitialized, 1);
            if (isInitialized == 1)
            {
                return;
            }
            
            Console.WriteLine("Initializing indexes...");

            try
            {
                var keyBuilder = Builders<RecordedEvent>.IndexKeys;

                var keys = keyBuilder
                    .Combine(
                        keyBuilder.Ascending(e => e.Stream),
                        keyBuilder.Ascending(e => e.EventNumber)
                    );

                var options = new CreateIndexOptions
                {
                    Unique = true
                };

                var model = new CreateIndexModel<RecordedEvent>(keys, options);
                await _events.Indexes.CreateOneAsync(model);
            }
            catch (Exception)
            {
                Interlocked.Exchange(ref _isInitialized, 0);
                throw;
            }
            
            Console.WriteLine("Indexes initialized");
        }
    }
}