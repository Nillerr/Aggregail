namespace EventSourcing.Demo.Framework
{
    public static class AggregateConfigurationExtensions
    {
        public static AggregateConfiguration<TAggregate> Applies<T, TAggregate>(
            this AggregateConfiguration<TAggregate> configuration,
            EventType<T> type
        )
            where TAggregate : Aggregate<TAggregate>, IApplies<T>
        {
            return configuration.Applies(type, (aggregate, @event) => aggregate.Apply(@event));
        }
    }
}