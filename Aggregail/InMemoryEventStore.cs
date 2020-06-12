using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Aggregail.InMemory;

namespace Aggregail
{
    /// <summary>
    /// A simple in-memory implementation of an event store.
    /// </summary>
    /// <remarks>
    /// This class is <b><i>not</i></b> thread-safe, and will fail if used on several threads simultaneously.
    /// </remarks>
    public sealed class InMemoryEventStore : IEventStore
    {
        private readonly Dictionary<string, List<StoredEvent>> _streams =
            new Dictionary<string, List<StoredEvent>>();

        private readonly Dictionary<string, List<StoredEvent>> _byEventType =
            new Dictionary<string, List<StoredEvent>>();

        private readonly Dictionary<string, List<StoredEvent>> _byCategory =
            new Dictionary<string, List<StoredEvent>>();

        private readonly IJsonEventSerializer _serializer;
        private readonly IStreamNameResolver _streamNameResolver;
        private readonly IClock _clock;
        private readonly IMetadataFactory _metadataFactory;

        /// <summary>
        /// Creates an instance of the <see cref="InMemoryEventStore"/> class.
        /// </summary>
        /// <param name="settings">Settings</param>
        public InMemoryEventStore(InMemoryEventStoreSettings settings)
        {
            _serializer = settings.EventSerializer;
            _streamNameResolver = settings.StreamNameResolver;
            _clock = settings.Clock;
            _metadataFactory = settings.MetadataFactory;
        }

        /// <inheritdoc />
        public Task AppendToStreamAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long expectedVersion,
            IEnumerable<IPendingEvent> pendingEvents,
            CancellationToken cancellationToken = default
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            ExpectedVersionValidation.ValidateExpectedVersion(expectedVersion);

            var stream = _streamNameResolver.Stream(id, configuration);

            var storedEvents = _streams.GetValueOrDefault(stream);
            var currentVersion = storedEvents?.Last().EventNumber;

            var startingVersion = ExpectedVersionValidation
                .StartingVersion(expectedVersion, currentVersion, stream);

            if (storedEvents == null)
            {
                storedEvents = new List<StoredEvent>();
                _streams[stream] = storedEvents;
            }

            if (startingVersion < storedEvents.Count)
            {
                // Validation phase
                ValidateExistingEvents(stream, expectedVersion, pendingEvents, startingVersion, storedEvents,
                    currentVersion
                );

                // Everything is already committed
            }
            else
            {
                var recordableEvents = ToStoredEvents(stream, pendingEvents, startingVersion);
                foreach (var recordableEvent in recordableEvents)
                {
                    storedEvents.Add(recordableEvent);
                    AddByEventType(recordableEvent);
                    AddByCategory(recordableEvent, configuration);
                }
            }

            return Task.CompletedTask;
        }

        private static void ValidateExistingEvents(
            string stream,
            long expectedVersion,
            IEnumerable<IPendingEvent> pendingEvents,
            long startingVersion,
            List<StoredEvent> storedEvents,
            long? currentVersion
        )
        {
            var pendingEventsList = pendingEvents.ToList();
            for (var pendingIndex = 0; pendingIndex < pendingEventsList.Count; pendingIndex++)
            {
                var storedIndex = (int) startingVersion + pendingIndex;
                if (storedIndex >= storedEvents.Count)
                {
                    throw new WrongExpectedVersionException(
                        $"Could not write to stream `{stream}`. Some of the events were not previously committed.",
                        expectedVersion, currentVersion
                    );
                }

                var pendingEvent = pendingEventsList[pendingIndex];
                var storedEvent = storedEvents[storedIndex];
                if (pendingEvent.Id != storedEvent.EventId)
                {
                    throw new WrongExpectedVersionException(
                        $"Could not write to stream `{stream}`. Some of the events were not previously committed.",
                        expectedVersion, currentVersion
                    );
                }
            }
        }

        private IEnumerable<StoredEvent> ToStoredEvents(
            string stream,
            IEnumerable<IPendingEvent> pendingEvents,
            long startingVersion
        )
        {
            return pendingEvents
                .Select((pendingEvent, index) => ToStoredEvent(stream, pendingEvent, startingVersion + index));
        }

        private StoredEvent ToStoredEvent(string stream, IPendingEvent pendingEvent, long eventVersion)
        {
            var data = pendingEvent.Data(_serializer);
            var metadata = _metadataFactory.MetadataFor(pendingEvent.Id, pendingEvent.Type, data, _serializer);
            var created = _clock.UtcNow;

            return new StoredEvent(
                pendingEvent.Id,
                stream,
                pendingEvent.Type,
                eventVersion,
                data,
                metadata,
                created
            );
        }

        private void AddByEventType(StoredEvent storedEvent)
        {
            if (_byEventType.TryGetValue(storedEvent.EventType, out var byEventType))
            {
                byEventType.Add(storedEvent);
            }
            else
            {
                byEventType = new List<StoredEvent>();
                byEventType.Add(storedEvent);

                _byEventType[storedEvent.EventType] = byEventType;
            }
        }

        private void AddByCategory(StoredEvent storedEvent, IAggregateConfiguration configuration)
        {
            var category = configuration.Name;
            if (_byCategory.TryGetValue(category, out var byCategory))
            {
                byCategory.Add(storedEvent);
            }
            else
            {
                byCategory = new List<StoredEvent>();
                byCategory.Add(storedEvent);

                _byCategory[category] = byCategory;
            }
        }

        /// <inheritdoc />
        public Task<TAggregate?> AggregateAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long? version = null,
            CancellationToken cancellationToken = default
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            var stream = _streamNameResolver.Stream(id, configuration);

            if (_streams.TryGetValue(stream, out var eventStream) && eventStream.Count > 0)
            {
                var aggregate = ConstructAggregate(id, configuration, eventStream.First());

                foreach (var storedEvent in eventStream.Skip(1))
                {
                    if (version.HasValue && storedEvent.EventNumber > version.Value)
                    {
                        break;
                    }

                    ApplyEvent(aggregate, configuration, storedEvent);
                }

                return Task.FromResult<TAggregate?>(aggregate);
            }

            return Task.FromResult<TAggregate?>(null);
        }

        private TAggregate ConstructAggregate<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            StoredEvent storedEvent
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            if (!configuration.Constructors.TryGetValue(storedEvent.EventType, out var constructor))
            {
                throw new InvalidOperationException(
                    $"Unrecognized construction event type: {storedEvent.EventType}"
                );
            }

            var aggregate = constructor(id, _serializer, storedEvent.Data);
            aggregate.Record(new RecordableEvent(storedEvent.EventNumber));
            return aggregate;
        }

        private void ApplyEvent<TIdentity, TAggregate>(
            TAggregate aggregate,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            StoredEvent storedEvent
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            if (configuration.Applicators.TryGetValue(storedEvent.EventType, out var applicator))
            {
                applicator(aggregate, _serializer, storedEvent.Data);
                aggregate.Record(new RecordableEvent(storedEvent.EventNumber));
            }
            else
            {
                throw new InvalidOperationException($"Unexpected recorded event type: {storedEvent.EventType}");
            }
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
                await Task.Yield();

                if (!_byEventType.TryGetValue(eventType, out var eventStream))
                {
                    continue;
                }

                foreach (var storedEvent in eventStream.Where(e => e.EventNumber == 0))
                {
                    var id = _streamNameResolver.ParseId(storedEvent.EventStreamId, configuration);
                    yield return id;
                }
            }
        }

        /// <inheritdoc />
        public Task DeleteAggregateAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long expectedVersion,
            CancellationToken cancellationToken = default
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            var stream = _streamNameResolver.Stream(id, configuration);
            if (_streams.TryGetValue(stream, out var recordedEvents))
            {
                foreach (var recordedEvent in recordedEvents)
                {
                    _byEventType[recordedEvent.EventType].Remove(recordedEvent);
                }
            }

            _streams.Remove(stream);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<bool> AggregateExistsAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            CancellationToken cancellationToken = default
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            var stream = _streamNameResolver.Stream(id, configuration);
            var exists = _streams.ContainsKey(stream);
            return Task.FromResult(exists);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IRecordedEvent<TIdentity, TAggregate>>
            ReadStreamEventsAsync<TIdentity, TAggregate>(
                AggregateConfiguration<TIdentity, TAggregate> configuration,
                long start,
                [EnumeratorCancellation] CancellationToken cancellationToken = default
            ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            if (_byCategory.TryGetValue(configuration.Name, out var storedEvents))
            {
                foreach (var (index, storedEvent) in storedEvents.Skip((int) start).Select((e, i) => (i, e)))
                {
                    await Task.Yield();

                    var id = _streamNameResolver.ParseId(storedEvent.EventStreamId, configuration);
                    var recordedEvent = new RecordedEvent<TIdentity, TAggregate>(storedEvent, _serializer, id, start + index);
                    yield return recordedEvent;
                }
            }
        }
    }
}