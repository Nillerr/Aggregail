using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSourcing.Demo.Framework
{
    public interface IEventStoreAppender
    {
        Task AppendToStreamAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long expectedVersion,
            IEnumerable<IPendingEvent> pendingEvents
        )
            where TAggregate : Aggregate<TIdentity, TAggregate>;
    }
}