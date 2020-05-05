using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSourcing.Demo.Framework
{
    public interface IEventStoreAppender
    {
        Task AppendToStreamAsync<TAggregate>(
            Guid id,
            AggregateConfiguration<TAggregate> configuration,
            long expectedVersion,
            IEnumerable<IPendingEvent> pendingEvents
        )
            where TAggregate : Aggregate<TAggregate>;
    }
}