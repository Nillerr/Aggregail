using System;
using System.Threading.Tasks;
using EventSourcing.Demo.Framework.Serialiazation;
using EventStore.ClientAPI;

namespace EventSourcing.Demo.Framework
{
    public static class EventStoreConnectionExtensions
    {
        public static async Task<TAggregate?> AggregateAsync<TAggregate, TCreatedEvent>(
            this IEventStoreConnection connection,
            Guid id,
            AggregateConfiguration<TAggregate, TCreatedEvent> configuration,
            IJsonDecoder decoder
        )
            where TAggregate : Aggregate<TAggregate, TCreatedEvent>
        {
            var stream = configuration.Name.Stream(id);
            
            var createdResult = await connection.ReadEventAsync(stream, StreamPosition.Start, false);
            if (createdResult.Event == null)
            {
                return null;
            }

            var createdResolvedEvent = createdResult.Event.Value;
            var createdRecordedEvent = createdResolvedEvent.Event;

            var createdEvent = decoder.Decode<TCreatedEvent>(createdRecordedEvent.Data);

            var aggregate = configuration.Constructor(id, createdEvent);
            aggregate.Record(createdRecordedEvent);

            var applicators = configuration.Applicators;
            
            StreamEventsSlice slice;
            do
            {
                const int sliceSize = 100;
                slice = await connection.ReadStreamEventsForwardAsync(stream, 1, sliceSize, false);
                
                foreach (var resolvedEvent in slice.Events)
                {
                    var recordedEvent = resolvedEvent.Event;
                    if (applicators.TryGetValue(recordedEvent.EventType, out var applicator))
                    {
                        applicator(aggregate, decoder, recordedEvent.Data);
                        aggregate.Record(recordedEvent);
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