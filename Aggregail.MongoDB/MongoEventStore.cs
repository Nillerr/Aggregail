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

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoEventStore"/> class.
        /// </summary>
        /// <param name="settings">The settings to configure behaviour of the instance.</param>
        public MongoEventStore(MongoEventStoreSettings settings)
        {
            _events = settings.Database.GetCollection<RecordedEvent>(settings.Collection);
            _serializer = settings.EventSerializer;
            _logger = settings.Logger;
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
            catch (Exception ex)
            {
                _logger?.LogCritical(ex, "Index initialization failed");
                Interlocked.Exchange(ref _isInitialized, 0);
                throw;
            }
            
            _logger?.LogDebug("Index initialized");
        }
    }
}