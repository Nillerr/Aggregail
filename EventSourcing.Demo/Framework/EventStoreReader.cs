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

        public async Task<TAggregate?> AggregateAsync<TAggregate, TCreatedEvent>(
            Guid id,
            AggregateConfiguration<TAggregate, TCreatedEvent> configuration
        )
            where TAggregate : Aggregate<TAggregate, TCreatedEvent>
        {
            var stream = configuration.Name.Stream(id);

            var createdResult = await _connection.ReadEventAsync(stream, StreamPosition.Start, false);
            if (createdResult.Event == null)
            {
                return null;
            }

            var createdResolvedEvent = createdResult.Event.Value;
            var createdRecordedEvent = createdResolvedEvent.Event;

            var createdEvent = _decoder.Decode<TCreatedEvent>(createdRecordedEvent.Data);

            var aggregate = configuration.Constructor(id, createdEvent);
            aggregate.Record(new RecordedEvent(createdRecordedEvent.EventNumber));

            var applicators = configuration.Applicators;

            const int sliceSize = 100;
            
            StreamEventsSlice slice = await _connection.ReadStreamEventsForwardAsync(stream, 1, sliceSize, false);
            while (!slice.IsEndOfStream)
            {
                foreach (var resolvedEvent in slice.Events)
                {
                    var recordedEvent = resolvedEvent.Event;
                    if (applicators.TryGetValue(recordedEvent.EventType, out var applicator))
                    {
                        applicator(aggregate, _decoder, recordedEvent.Data);
                        aggregate.Record(new RecordedEvent(recordedEvent.EventNumber));
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unexpected recorded event type: {recordedEvent.EventType}"
                        );
                    }
                }

                slice = await _connection.ReadStreamEventsForwardAsync(stream, slice.NextEventNumber, sliceSize, false);
            }

            return aggregate;
        }
    }
}