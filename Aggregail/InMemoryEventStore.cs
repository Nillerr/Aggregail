using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

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
        private sealed class StoredEvent
        {
            public StoredEvent(Guid eventId, string eventStreamId, string eventType, long eventNumber, byte[] data)
            {
                EventId = eventId;
                EventStreamId = eventStreamId;
                EventType = eventType;
                EventNumber = eventNumber;
                Data = data;
            }

            public Guid EventId { get; }
            public string EventStreamId { get; }
            public string EventType { get; }
            public long EventNumber { get; }
            public byte[] Data { get; }
        }

        private readonly Dictionary<string, List<StoredEvent>> _streams = 
            new Dictionary<string, List<StoredEvent>>();

        private readonly Dictionary<string, List<StoredEvent>> _byEventType =
            new Dictionary<string, List<StoredEvent>>();

        private readonly IJsonEventSerializer _serializer;
        private readonly IStreamNameResolver _streamNameResolver;

        /// <summary>
        /// Creates an instance of the <see cref="InMemoryEventStore"/> class.
        /// </summary>
        /// <param name="settings">Settings</param>
        public InMemoryEventStore(InMemoryEventStoreSettings settings)
        {
            _serializer = settings.EventSerializer;
            _streamNameResolver = settings.StreamNameResolver;
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
                ValidateExistingEvents(stream, expectedVersion, pendingEvents, startingVersion, storedEvents, currentVersion);

                // Everything is already committed
            }
            else
            {
                var recordableEvents = ToStoredEvents(stream, pendingEvents, startingVersion);
                foreach (var recordableEvent in recordableEvents)
                {
                    storedEvents.Add(recordableEvent);
                    AddByEventType(recordableEvent);
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
            long currentVersion
        )
        {
            return pendingEvents
                .Select((pendingEvent, index) => ToStoredEvent(stream, pendingEvent, currentVersion + 1 + index));
        }

        private StoredEvent ToStoredEvent(string stream, IPendingEvent pendingEvent, long eventVersion)
        {
            var data = pendingEvent.Data(_serializer);
            
            return new StoredEvent(
                pendingEvent.Id,
                stream,
                pendingEvent.Type,
                eventVersion,
                data
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
                
                foreach (var storedEvent in eventStream)
                {
                    if (storedEvent.EventNumber != 0)
                    {
                        continue;
                    }
                    
                    var eventStreamParts = storedEvent.EventStreamId.Split("-", 2);
                    var lastEventStreamPart = eventStreamParts[1];
                    var id = configuration.IdentityParser(lastEventStreamPart);
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
    }
}