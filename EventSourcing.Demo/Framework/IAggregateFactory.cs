using System;

namespace EventSourcing.Demo.Framework
{
    public interface IAggregateFactory<out TAggregate, in TCreateEvent>
        where TAggregate : Aggregate<TAggregate, TCreateEvent>
    {
        TAggregate Create(Guid id, TCreateEvent @event);
    }
}