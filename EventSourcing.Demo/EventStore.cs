using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aggregail;
using EventStore.ClientAPI;

namespace EventSourcing.Demo
{
    public sealed class EventStore : IEventStore
    {
        private readonly IEventStoreConnection _connection;
        private readonly IJsonEventSerializer _serializer;

        public EventStore(IEventStoreConnection connection, IJsonEventSerializer serializer)
        {
            _connection = connection;
            _serializer = serializer;
        }

        public async Task AppendToStreamAsync<TIdentity>(
            TIdentity id,
            IAggregateConfiguration<TIdentity> configuration,
            long expectedVersion,
            IEnumerable<IPendingEvent> pendingEvents
        )
        {
            var events = pendingEvents
                .Select(EventData)
                .ToArray();

            var stream = configuration.Stream(id);
            await _connection.AppendToStreamAsync(stream, expectedVersion, events);
        }

        private EventData EventData(IPendingEvent pendingEvent)
        {
            var data = pendingEvent.Data(_serializer);
            return new EventData(pendingEvent.Id, pendingEvent.Type, true, data, null);
        }

        public async Task<TAggregate?> AggregateAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration
        )
            where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            var stream = configuration.Name.Stream(id);

            var createdResult = await _connection.ReadEventAsync(stream, StreamPosition.Start, false);
            if (createdResult.Event == null)
            {
                return null;
            }

            var createdResolvedEvent = createdResult.Event.Value;
            var createdRecordedEvent = createdResolvedEvent.Event;

            if (!configuration.Constructors.TryGetValue(createdRecordedEvent.EventType, out var constructor))
            {
                throw new InvalidOperationException($"Unrecognized construction event type: {createdRecordedEvent.EventType}");
            }

            var aggregate = constructor(id, _serializer, createdRecordedEvent.Data);
            aggregate.Record(new RecordableEvent(createdRecordedEvent.EventNumber));

            var applicators = configuration.Applicators;

            long sliceStart = 1;
            const int sliceSize = 100;
            
            StreamEventsSlice slice; 
            do
            {
                slice = await _connection.ReadStreamEventsForwardAsync(stream, sliceStart, sliceSize, false);
                
                foreach (var resolvedEvent in slice.Events)
                {
                    var recordedEvent = resolvedEvent.Event;
                    if (applicators.TryGetValue(recordedEvent.EventType, out var applicator))
                    {
                        applicator(aggregate, _serializer, recordedEvent.Data);
                        aggregate.Record(new RecordableEvent(recordedEvent.EventNumber));
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unexpected recorded event type: {recordedEvent.EventType}");
                    }
                }

                sliceStart = slice.NextEventNumber;
            } while (!slice.IsEndOfStream);

            return aggregate;
        }

        public IAsyncEnumerable<TIdentity> AggregateIdsAsync<TIdentity, TAggregate>(AggregateConfiguration<TIdentity, TAggregate> configuration) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            throw new NotImplementedException();
        }
    }
}