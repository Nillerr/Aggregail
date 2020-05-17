namespace Aggregail
{
    /// <summary>
    /// Extension methods for <see cref="AggregateConfiguration{TIdentity,TAggregate}"/>.
    /// </summary>
    public static class AggregateConfigurationExtensions
    {
        /// <summary>
        /// Configures an applicator for the event of type <typeparamref name="T"/>, identified by the
        /// <paramref name="type"/> argument, calling the implemented <see cref="IApplies{TEvent}.Apply"/> method with
        /// the deserialized event data of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="configuration">The aggregate configuration.</param>
        /// <param name="type">Type of the event.</param>
        /// <typeparam name="T">Type of event.</typeparam>
        /// <typeparam name="TIdentity">Type of ID of the aggregate.</typeparam>
        /// <typeparam name="TAggregate">Type of aggregate.</typeparam>
        /// <returns>The modified aggregate configuration.</returns>
        public static AggregateConfiguration<TIdentity, TAggregate> Applies<T, TIdentity, TAggregate>(
            this AggregateConfiguration<TIdentity, TAggregate> configuration,
            EventType<T> type
        )
            where TAggregate : Aggregate<TIdentity, TAggregate>, IApplies<T>
            where T : class
        {
            return configuration.Applies(type, (aggregate, @event) => aggregate.Apply(@event));
        }
    }
}