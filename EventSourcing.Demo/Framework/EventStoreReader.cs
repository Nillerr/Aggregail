using System;
using System.Threading.Tasks;
using EventSourcing.Demo.Framework.Serialiazation;
using EventStore.ClientAPI;

namespace EventSourcing.Demo.Framework
{
    public sealed class EventStoreReader : IEventStoreReader
    {
        private readonly IEventStoreConnection _connection;
        private readonly IJsonDecoder _decoder;

        public EventStoreReader(IEventStoreConnection connection, IJsonDecoder decoder)
        {
            _connection = connection;
            _decoder = decoder;
        }

        public async Task<TAggregate?> AggregateAsync<TAggregate>(
            Guid id,
            AggregateConfiguration<TAggregate> configuration
        )
            where TAggregate : Aggregate<TAggregate>
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

            var aggregate = constructor(id, _decoder, createdRecordedEvent.Data);
            aggregate.Record(new RecordableEvent(createdRecordedEvent.EventNumber));

            var applicators = configuration.Applicators;

            const int sliceSize = 100;
            
            StreamEventsSlice slice; 
            do
            {
                slice = await _connection.ReadStreamEventsForwardAsync(stream, 1, sliceSize, false);
                
                foreach (var resolvedEvent in slice.Events)
                {
                    var recordedEvent = resolvedEvent.Event;
                    if (applicators.TryGetValue(recordedEvent.EventType, out var applicator))
                    {
                        applicator(aggregate, _decoder, recordedEvent.Data);
                        aggregate.Record(new RecordableEvent(recordedEvent.EventNumber));
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unexpected recorded event type: {recordedEvent.EventType}");
                    }
                }
            } while (!slice.IsEndOfStream);

            return aggregate;
        }
    }
}