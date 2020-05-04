using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSourcing.Demo.Framework
{
    public interface IEventStoreAppender
    {
        Task AppendToStreamAsync<TAggregate, TCreateEvent>(
            Guid id,
            AggregateConfiguration<TAggregate, TCreateEvent> configuration,
            long expectedVersion,
            IEnumerable<IPendingEvent> pendingEvents
        )
            where TAggregate : Aggregate<TAggregate, TCreateEvent>;
    }
}