using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aggregail
{
    public interface IEventStore
    {
        Task AppendToStreamAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long expectedVersion,
            IEnumerable<IPendingEvent> pendingEvents
        )
            where TAggregate : Aggregate<TIdentity, TAggregate>;
        
        Task<TAggregate?> AggregateAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration
        )
            where TAggregate : Aggregate<TIdentity, TAggregate>;
    }
}