using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aggregail
{
    public sealed class InMemoryEventStore : IEventStore
    {
        private sealed class StoredEvent
        {
            public StoredEvent(Guid eventId, string eventType, long eventNumber, byte[] data)
            {
                EventId = eventId;
                EventType = eventType;
                EventNumber = eventNumber;
                Data = data;
            }

            public Guid EventId { get; }
            public string EventType { get; }
            public long EventNumber { get; }
            public byte[] Data { get; }
        }

        private readonly Dictionary<string, List<StoredEvent>> _streams = new Dictionary<string, List<StoredEvent>>();

        private readonly IJsonEventSerializer _serializer;

        public InMemoryEventStore(IJsonEventSerializer serializer)
        {
            _serializer = serializer;
        }

        public Task AppendToStreamAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long expectedVersion,
            IEnumerable<IPendingEvent> pendingEvents
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            var stream = configuration.Name.Stream(id);

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

                    var storedEvents = ToStoredEvents(pendingEvents, ExpectedVersion.NoStream);
                    foreach (var storedEvent in storedEvents)
                    {
                        eventStream.Add(storedEvent);
                    }

                    _streams[stream] = eventStream;

                    break;
                }
                default:
                {
                    if (_streams.TryGetValue(stream, out var eventStream) && eventStream.Count > 0)
                    {
                        var currentVersion = eventStream.Last().EventNumber;

                        var storedEvents = ToStoredEvents(pendingEvents, currentVersion);
                        foreach (var storedEvent in storedEvents)
                        {
                            eventStream.Add(storedEvent);
                        }
                    }

                    break;
                }
            }

            return Task.CompletedTask;
        }

        private IEnumerable<StoredEvent> ToStoredEvents(
            IEnumerable<IPendingEvent> pendingEvents,
            long currentVersion
        ) =>
            pendingEvents.Select((pendingEvent, index) => ToStoredEvent(pendingEvent, currentVersion + 1 + index));

        private StoredEvent ToStoredEvent(IPendingEvent pendingEvent, long eventVersion) =>
            new StoredEvent(pendingEvent.Id, pendingEvent.Type, eventVersion, pendingEvent.Data(_serializer));

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
                    throw new InvalidOperationException($"Unrecognized construction event type: {createStoredEvent.EventType}");
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
    }
}