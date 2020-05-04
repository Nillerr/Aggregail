namespace EventSourcing.Demo.Framework
{
    public static class AggregateConfigurationExtensions
    {
        public static AggregateConfiguration<TAggregate, TCreatedEvent> Applies<T, TAggregate, TCreatedEvent>(
            this AggregateConfiguration<TAggregate, TCreatedEvent> configuration,
            EventType<T> type
        )
            where TAggregate : Aggregate<TAggregate, TCreatedEvent>, IApply<T>
        {
            return configuration.Apply(type, (aggregate, @event) => aggregate.Apply(@event));
        }
    }
}