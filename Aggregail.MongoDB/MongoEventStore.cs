using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
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
        private readonly IMongoDatabase _database;
        private readonly string _collection;

        private readonly IMongoCollection<RecordedEvent> _events;
        private readonly IJsonEventSerializer _serializer;
        private readonly ILogger<MongoEventStore>? _logger;
        private readonly TransactionOptions _transactionOptions;
        private readonly IClock _clock;
        private readonly IStreamNameResolver _streamNameResolver;
        private readonly IMetadataFactory _metadataFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoEventStore"/> class.
        /// </summary>
        /// <param name="settings">The settings to configure behaviour of the instance.</param>
        public MongoEventStore(MongoEventStoreSettings settings)
        {
            _database = settings.Database;
            _collection = settings.Collection;

            _events = settings.Database.GetCollection<RecordedEvent>(settings.Collection);
            _serializer = settings.EventSerializer;
            _logger = settings.Logger;
            _clock = settings.Clock;
            _transactionOptions = settings.TransactionOptions;
            _streamNameResolver = settings.StreamNameResolver;
            _metadataFactory = settings.MetadataFactory;
        }

        /// <inheritdoc />
        public async Task AppendToStreamAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long expectedVersion,
            IEnumerable<IPendingEvent> pendingEvents,
            CancellationToken cancellationToken = default
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            ExpectedVersionValidation.ValidateExpectedVersion(expectedVersion);

            var stream = _streamNameResolver.Stream(id, configuration);

            using var session = await _events.Database.Client.StartSessionAsync(null, cancellationToken);

            session.StartTransaction(_transactionOptions);

            try
            {
                var latestEvent = await _events
                    .Find(session, e => e.Stream == stream)
                    .SortByDescending(e => e.EventNumber)
                    .FirstOrDefaultAsync(cancellationToken);

                var currentVersion = latestEvent?.EventNumber;
                var startingVersion = ExpectedVersionValidation
                    .StartingVersion(expectedVersion, currentVersion, stream);

                var recordedEvents = pendingEvents
                    .Select((pendingEvent, index) => RecordedEvent(stream, pendingEvent, startingVersion + index))
                    .ToArray();

                try
                {
                    await _events.InsertManyAsync(session, recordedEvents, null, cancellationToken);
                    await session.CommitTransactionAsync(cancellationToken);
                }
                catch (MongoWriteException ex)
                {
                    if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                    {
                        throw new WrongExpectedVersionException(
                            "Either none or only a subset of the events were previously committed.",
                            expectedVersion, currentVersion
                        );
                    }

                    throw;
                }
            }
            catch (Exception)
            {
                // ReSharper disable once MethodSupportsCancellation
                await session.AbortTransactionAsync();
                throw;
            }
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
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long? version = null,
            CancellationToken cancellationToken = default
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            using var session = await _events.Database.Client.StartSessionAsync(null, cancellationToken);

            var stream = _streamNameResolver.Stream(id, configuration);

            var fb = Builders<RecordedEvent>.Filter;
            var filter = version == null
                ? fb.Where(e => e.Stream == stream)
                : fb.Where(e => e.Stream == stream && e.EventNumber <= version.Value);
            
            var cursor = await _events
                .Find(filter)
                .SortBy(e => e.EventNumber)
                .ToCursorAsync(cancellationToken);

            TAggregate? aggregate = null;

            while (await cursor.MoveNextAsync(cancellationToken))
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
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            [EnumeratorCancellation] CancellationToken cancellationToken = default
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            var constructors = configuration.Constructors;
            foreach (var (eventType, _) in constructors)
            {
                var cursor = await _events
                    .Find(e => e.EventType == eventType && e.EventNumber == 0)
                    .ToCursorAsync(cancellationToken);

                while (await cursor.MoveNextAsync(cancellationToken))
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

        /// <inheritdoc />
        public async Task DeleteAggregateAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long expectedVersion,
            CancellationToken cancellationToken = default
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            using var session = await _database.Client.StartSessionAsync(cancellationToken: cancellationToken);

            session.StartTransaction();

            var stream = _streamNameResolver.Stream(id, configuration);

            if (expectedVersion != ExpectedVersion.Any && expectedVersion != ExpectedVersion.NoStream)
            {
                var latestEvent = await _events
                    .Find(session, e => e.Stream == stream)
                    .SortByDescending(e => e.EventNumber)
                    .FirstOrDefaultAsync(cancellationToken);

                if (latestEvent == null)
                {
                    throw new WrongExpectedVersionException(
                        $"Expected stream `{stream}` to be at version {expectedVersion}, but the stream did not exist.",
                        expectedVersion, null
                    );
                }
            }

            await _events.DeleteManyAsync(session, e => e.Stream == stream, null, cancellationToken);

            await session.CommitTransactionAsync(cancellationToken);
        }

        /// <summary>
        /// Initializes the event store, creating the collection in case it's missing, and sets up indexes on the
        /// collection.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>The task of the async operation</returns>
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            _logger?.LogDebug($"Initializing collection `{_collection}`...");

            await InitializeCollection(cancellationToken);

            _logger?.LogDebug("Initializing indexes...");

            await CreateStreamEventNumberIndexAsync(cancellationToken);
            await CreateEventTypeIndexAsync(cancellationToken);
            await CreateEventNumberCreatedIndexAsync(cancellationToken);
            await CreateCreatedIndexAsync(cancellationToken);

            _logger?.LogDebug("Indexes initialized successfully");

            _logger?.LogDebug($"Collection `{_collection}` initialized successfully");
        }

        private async Task InitializeCollection(CancellationToken cancellationToken)
        {
            var options = new ListCollectionNamesOptions();

            options.Filter = Builders<BsonDocument>.Filter
                .Where(e => e["name"] == _collection);

            var collectionNamesCursor = await _database.ListCollectionNamesAsync(options, cancellationToken);
            var collectionName = await collectionNamesCursor.FirstOrDefaultAsync(cancellationToken);
            if (collectionName == null)
            {
                _logger?.LogDebug($"Creating collection `{_collection}`...");
                await _database.CreateCollectionAsync(_collection, cancellationToken: cancellationToken);
                _logger?.LogDebug($"Collection `{_collection}` created successfully");
            }
            else
            {
                _logger?.LogTrace($"Collection `{_collection}` already exists");
            }
        }

        private async Task CreateStreamEventNumberIndexAsync(CancellationToken cancellationToken)
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
            await _events.Indexes.CreateOneAsync(model, cancellationToken: cancellationToken);
        }

        private async Task CreateEventTypeIndexAsync(CancellationToken cancellationToken)
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
            await _events.Indexes.CreateOneAsync(model, cancellationToken: cancellationToken);
        }

        private async Task CreateEventNumberCreatedIndexAsync(CancellationToken cancellationToken)
        {
            var keyBuilder = Builders<RecordedEvent>.IndexKeys;

            var keys = keyBuilder.Combine(
                keyBuilder.Ascending(e => e.EventNumber),
                keyBuilder.Ascending(e => e.Created)
            );

            var options = new CreateIndexOptions();
            options.Background = true;

            var model = new CreateIndexModel<RecordedEvent>(keys, options);
            await _events.Indexes.CreateOneAsync(model, cancellationToken: cancellationToken);
        }

        private async Task CreateCreatedIndexAsync(CancellationToken cancellationToken)
        {
            var keyBuilder = Builders<RecordedEvent>.IndexKeys;

            var keys = keyBuilder.Ascending(e => e.Created);

            var options = new CreateIndexOptions();
            options.Background = true;

            var model = new CreateIndexModel<RecordedEvent>(keys, options);
            await _events.Indexes.CreateOneAsync(model, cancellationToken: cancellationToken);
        }
    }
}