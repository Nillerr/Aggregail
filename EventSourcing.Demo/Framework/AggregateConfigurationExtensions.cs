namespace EventSourcing.Demo.Framework
{
    public static class AggregateConfigurationExtensions
    {
        public static AggregateConfiguration<TAggregate, TCreatedEvent> Applies<T, TAggregate, TCreatedEvent>(
            this AggregateConfiguration<TAggregate, TCreatedEvent> configuration,
            EventType<T> type
        )
            where TAggregate : Aggregate<TAggregate, TCreatedEvent>, IApplies<T>
        {
            return configuration.Applies(type, (aggregate, @event) => aggregate.Apply(@event));
        }
    }
}