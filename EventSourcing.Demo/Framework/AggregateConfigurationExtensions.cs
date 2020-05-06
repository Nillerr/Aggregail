namespace EventSourcing.Demo.Framework
{
    public static class AggregateConfigurationExtensions
    {
        public static AggregateConfiguration<TIdentity, TAggregate> Applies<T, TIdentity, TAggregate>(
            this AggregateConfiguration<TIdentity, TAggregate> configuration,
            EventType<T> type
        )
            where TAggregate : Aggregate<TIdentity, TAggregate>, IApplies<T>
        {
            return configuration.Applies(type, (aggregate, @event) => aggregate.Apply(@event));
        }
    }
}