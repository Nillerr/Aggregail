﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Aggregail.MongoDB
{
    /// <summary>
    /// An event store driver using MongoDB as its underlying storage.
    /// </summary>
    /// <remarks>
    /// It is strongly recommended to leave the default <see cref="MongoEventStoreSettings.TransactionOptions"/>, and
    /// configure the MongoDB connection with <see cref="ReadPreference"/> set to <see cref="ReadPreference.Primary"/>
    /// (which is the default).
    /// </remarks>
    public sealed class MongoEventStore : IEventStore
    {
        private static int _isInitialized;

        private readonly IMongoCollection<RecordedEvent> _events;
        private readonly IJsonEventSerializer _serializer;
        private readonly ILogger<MongoEventStore>? _logger;
        private readonly TransactionOptions _transactionOptions;
        private readonly IClock _clock;
        private readonly IMetadataFactory _metadataFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoEventStore"/> class.
        /// </summary>
        /// <param name="settings">The settings to configure behaviour of the instance.</param>
        public MongoEventStore(MongoEventStoreSettings settings)
        {
            _events = settings.Database.GetCollection<RecordedEvent>(settings.Collection);
            _serializer = settings.EventSerializer;
            _logger = settings.Logger;
            _clock = settings.Clock;
            _transactionOptions = settings.TransactionOptions;
        }

        /// <inheritdoc />
        public async Task AppendToStreamAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long expectedVersion,
            IEnumerable<IPendingEvent> pendingEvents
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            await InitializeIndexesAsync();
            
            using var session = await _events.Database.Client.StartSessionAsync();
            
            session.StartTransaction(_transactionOptions);

            var stream = configuration.Name.Stream(id);

            var latestEvent = await _events
                .Find(session, e => e.Stream == stream)
                .SortByDescending(e => e.EventNumber)
                .FirstOrDefaultAsync();

            if (expectedVersion == ExpectedVersion.NoStream && latestEvent != null)
            {
                throw new WrongExpectedVersionException($"Expected stream `{stream}` to not exist, but did exist at version {latestEvent.EventNumber}.", expectedVersion, latestEvent.EventNumber);
            }

            if (expectedVersion > ExpectedVersion.NoStream && latestEvent == null)
            {
                throw new WrongExpectedVersionException($"Expected stream `{stream}` to be at version {expectedVersion}, but stream did not exist yet.", expectedVersion, null);
            }

            var currentVersion = latestEvent?.EventNumber ?? -1L;

            var recordedEvents = pendingEvents
                .Select((pendingEvent, index) => RecordedEvent(stream, pendingEvent, currentVersion + index + 1))
                .ToArray();

            try
            {
                await _events.InsertManyAsync(session, recordedEvents);
            }
            catch (MongoWriteException)
            {
                await session.AbortTransactionAsync();
                throw;
            }

            await session.CommitTransactionAsync();
        }

        private RecordedEvent RecordedEvent(string stream, IPendingEvent pendingEvent, long eventNumber)
        {
            var e = new RecordedEvent();
            e.Stream = stream;
            e.EventId = pendingEvent.Id;
            e.EventType = pendingEvent.Type;
            e.EventNumber = eventNumber;
            e.Created = UtcNow;
            e.Data = pendingEvent.Data(_serializer);
            e.Metadata = pendingEvent.Metadata(_metadataFactory, _serializer);
            return e;
        }

        private DateTime UtcNow
        {
            get
            {
                var created = _clock.UtcNow;
                if (created.Kind != DateTimeKind.Utc)
                {
                    throw new InvalidOperationException("The clock returned a timestamp which was not in UTC.");
                }

                return created;
            }
        }

        /// <inheritdoc />
        public async Task<TAggregate?> AggregateAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            await InitializeIndexesAsync();

            using var session = await _events.Database.Client.StartSessionAsync();

            var stream = configuration.Name.Stream(id);
    
            var cursor = await _events
                .Find(e => e.Stream == stream)
                .SortBy(e => e.EventNumber)
                .ToCursorAsync();

            TAggregate? aggregate = null;
            
            while (await cursor.MoveNextAsync())
            {
                foreach (var recordedEvent in cursor.Current)
                {
                    if (aggregate == null)
                    {
                        aggregate = ConstructAggregate(id, configuration, recordedEvent);
                    }
                    else
                    {
                        ApplyEvent(aggregate, configuration, recordedEvent);
                    }
                }
            }

            return aggregate;
        }

        private TAggregate ConstructAggregate<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            RecordedEvent recordedEvent
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            var constructors = configuration.Constructors;

            if (!constructors.TryGetValue(recordedEvent.EventType, out var constructor))
            {
                throw new InvalidOperationException($"Unrecognized construction event type: {recordedEvent.EventType}");
            }

            var aggregate = constructor(id, _serializer, recordedEvent.Data ?? Array.Empty<byte>());

            var recordableEvent = new RecordableEvent(recordedEvent.EventNumber);
            aggregate.Record(recordableEvent);

            return aggregate;
        }

        private void ApplyEvent<TIdentity, TAggregate>(
            TAggregate aggregate,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            RecordedEvent recordedEvent
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            var applicators = configuration.Applicators;

            if (!applicators.TryGetValue(recordedEvent.EventType, out var applicator))
            {
                throw new InvalidOperationException($"Unexpected recorded event type: {recordedEvent.EventType}");
            }

            applicator(aggregate, _serializer, recordedEvent.Data ?? Array.Empty<byte>());

            var recordableEvent = new RecordableEvent(recordedEvent.EventNumber);
            aggregate.Record(recordableEvent);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<TIdentity> AggregateIdsAsync<TIdentity, TAggregate>(
            AggregateConfiguration<TIdentity, TAggregate> configuration
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            var constructors = configuration.Constructors;
            foreach (var (eventType, _) in constructors)
            {
                var cursor = await _events
                    .Find(e => e.EventType == eventType && e.EventNumber == 0)
                    .ToCursorAsync();

                while (await cursor.MoveNextAsync())
                {
                    foreach (var recordedEvent in cursor.Current)
                    {
                        var eventStreamParts = recordedEvent.Stream.Split("-", 2);
                        var lastEventStreamPart = eventStreamParts[1];
                        var id = configuration.IdentityParser(lastEventStreamPart);
                        yield return id;
                    }
                }
            }
        }

        private async Task InitializeIndexesAsync()
        {
            var isInitialized = Interlocked.Exchange(ref _isInitialized, 1);
            if (isInitialized == 1)
            {
                return;
            }
            
            _logger?.LogDebug("Initializing index...");

            try
            {
                await CreateStreamEventNumberIndexAsync();
                await CreateEventTypeIndexAsync();
                await CreateEventNumberCreatedIndexAsync();
                await CreateCreatedIndexAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogCritical(ex, "Index initialization failed");
                Interlocked.Exchange(ref _isInitialized, 0);
                throw;
            }
            
            _logger?.LogDebug("Index initialized");
        }

        private async Task CreateStreamEventNumberIndexAsync()
        {
            var keyBuilder = Builders<RecordedEvent>.IndexKeys;

            var keys = keyBuilder
                .Combine(
                    keyBuilder.Ascending(e => e.Stream),
                    keyBuilder.Ascending(e => e.EventNumber)
                );

            var options = new CreateIndexOptions();
            options.Unique = true;

            var model = new CreateIndexModel<RecordedEvent>(keys, options);
            await _events.Indexes.CreateOneAsync(model);
        }

        private async Task CreateEventTypeIndexAsync()
        {
            var keyBuilder = Builders<RecordedEvent>.IndexKeys;

            var keys = keyBuilder
                .Combine(
                    keyBuilder.Ascending(e => e.EventType),
                    keyBuilder.Ascending(e => e.EventNumber)
                );

            var options = new CreateIndexOptions();
            options.Unique = false;

            var model = new CreateIndexModel<RecordedEvent>(keys, options);
            await _events.Indexes.CreateOneAsync(model);
        }

        private async Task CreateEventNumberCreatedIndexAsync()
        {
            var keyBuilder = Builders<RecordedEvent>.IndexKeys;

            var keys = keyBuilder.Combine(
                keyBuilder.Ascending(e => e.EventNumber),
                keyBuilder.Ascending(e => e.Created)
            );

            var options = new CreateIndexOptions();
            options.Background = true;

            var model = new CreateIndexModel<RecordedEvent>(keys, options);
            await _events.Indexes.CreateOneAsync(model);
        }

        private async Task CreateCreatedIndexAsync()
        {
            var keyBuilder = Builders<RecordedEvent>.IndexKeys;

            var keys = keyBuilder.Ascending(e => e.Created);

            var options = new CreateIndexOptions();
            options.Background = true;

            var model = new CreateIndexModel<RecordedEvent>(keys, options);
            await _events.Indexes.CreateOneAsync(model);
        }
    }
}