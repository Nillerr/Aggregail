using System;
using System.Threading.Tasks;

namespace EventSourcing.Demo.Framework
{
    public interface IEventStoreReader
    {
        Task<TAggregate?> AggregateAsync<TAggregate>(
            Guid id,
            AggregateConfiguration<TAggregate> configuration
        )
            where TAggregate : Aggregate<TAggregate>;
    }
}