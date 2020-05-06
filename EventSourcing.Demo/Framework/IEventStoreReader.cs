using System.Threading.Tasks;

namespace EventSourcing.Demo.Framework
{
    public interface IEventStoreReader
    {
        Task<TAggregate?> AggregateAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration
        )
            where TAggregate : Aggregate<TIdentity, TAggregate>;
    }
}