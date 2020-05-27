using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Creates an instance of the <see cref="InMemoryEventStore"/> class.
        /// </summary>
        /// <param name="serializer">The event serializer.</param>
        public InMemoryEventStore(IJsonEventSerializer serializer)
        {
            _serializer = serializer;
        }

        /// <inheritdoc />
        public Task AppendToStreamAsync<TIdentity>(
            TIdentity id,
            IAggregateConfiguration<TIdentity> configuration,
            long expectedVersion,
            IEnumerable<IPendingEvent> pendingEvents
        )
        {
            var stream = configuration.Stream(id);

            switch (expectedVersion)
            {
                case ExpectedVersion.NoStream:
                {
                    if (_streams.TryGetValue(stream, out var eventStream) && eventStream.Count > 0)
                    {
                        var actualVersion = eventStream.LastOrDefault()?.EventNumber;
                        throw new WrongExpectedVersionException("", ExpectedVersion.NoStream, actualVersion);
                    }

                    eventStream = new List<StoredEvent>();

                    var storedEvents = ToStoredEvents(stream, pendingEvents, ExpectedVersion.NoStream);
                    foreach (var storedEvent in storedEvents)
                    {
                        eventStream.Add(storedEvent);
                        AddByEventType(storedEvent);
                    }

                    _streams[stream] = eventStream;

                    break;
                }
                default:
                {
                    if (_streams.TryGetValue(stream, out var eventStream) && eventStream.Count > 0)
                    {
                        var currentVersion = eventStream.Last().EventNumber;

                        var storedEvents = ToStoredEvents(stream, pendingEvents, currentVersion);
                        foreach (var storedEvent in storedEvents)
                        {
                            eventStream.Add(storedEvent);
                            AddByEventType(storedEvent);
                        }
                    }

                    break;
                }
            }

            return Task.CompletedTask;
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
            AggregateConfiguration<TIdentity, TAggregate> configuration
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            var stream = configuration.Name.Stream(id);

            if (_streams.TryGetValue(stream, out var eventStream) && eventStream.Count > 0)
            {
                var createStoredEvent = eventStream.First();
                if (!configuration.Constructors.TryGetValue(createStoredEvent.EventType, out var constructor))
                {
                    throw new InvalidOperationException(
                        $"Unrecognized construction event type: {createStoredEvent.EventType}"
                    );
                }

                var aggregate = constructor(id, _serializer, createStoredEvent.Data);
                aggregate.Record(new RecordableEvent(createStoredEvent.EventNumber));

                foreach (var storedEvent in eventStream.Skip(1))
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

                return Task.FromResult<TAggregate?>(aggregate);
            }

            return Task.FromResult<TAggregate?>(null);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<TIdentity> AggregateIdsAsync<TIdentity, TAggregate>(
            AggregateConfiguration<TIdentity, TAggregate> configuration
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            await Task.Yield();
            
            var constructors = configuration.Constructors;
            foreach (var (eventType, _) in constructors)
            {
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
    }
}