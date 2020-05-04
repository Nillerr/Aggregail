using System;
using System.Threading.Tasks;

namespace EventSourcing.Demo.Framework
{
    public interface IEventStoreReader
    {
        Task<TAggregate?> AggregateAsync<TAggregate, TCreatedEvent>(
            Guid id,
            AggregateConfiguration<TAggregate, TCreatedEvent> configuration
        )
            where TAggregate : Aggregate<TAggregate, TCreatedEvent>;
    }
}