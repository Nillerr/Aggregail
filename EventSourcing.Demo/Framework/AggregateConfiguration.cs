using System;
using System.Collections.Immutable;

namespace EventSourcing.Demo.Framework
{
    public sealed class AggregateConfiguration<TAggregate, TCreatedEvent>
        where TAggregate : Aggregate<TAggregate, TCreatedEvent>
    {
        public AggregateName<TAggregate> Name { get; }
        
        public Func<Guid, TCreatedEvent, TAggregate> Constructor { get; }

        public ImmutableDictionary<string, EventApplicator<TAggregate>> Applicators { get; private set; } =
            ImmutableDictionary.Create<string, EventApplicator<TAggregate>>();

        public AggregateConfiguration(AggregateName<TAggregate> name, Func<Guid, TCreatedEvent, TAggregate> constructor)
        {
            Name = name;
            Constructor = constructor;
        }

        public AggregateConfiguration<TAggregate, TCreatedEvent> Apply<T>(EventType<T> type, Action<TAggregate, T> applicator)
        {
            Applicators = Applicators.Add(type.Value, (aggregate, decoder, data) =>
            {
                var @event = decoder.Decode<T>(data);
                applicator(aggregate, @event);
            });

            return this;
        }
    }
}