namespace Aggregail
{
    /// <summary>
    /// Resolves and parses the name of the stream for aggregates.
    /// </summary>
    public interface IStreamNameResolver
    {
        /// <summary>
        /// Resolves the name of the stream associated with the aggregate of type <typeparamref name="TAggregate"/>,
        /// identified by <paramref name="id"/>, and configured using <paramref name="configuration"/>.
        /// </summary>
        /// <param name="id">Id of the aggregate.</param>
        /// <param name="configuration">Configuration of the aggregate.</param>
        /// <typeparam name="TIdentity">Type of aggregate ID</typeparam>
        /// <typeparam name="TAggregate">Type of aggregate</typeparam>
        /// <returns>The name of the stream for the specified aggregate id and configuration.</returns>
        string Stream<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration
        )
            where TAggregate : Aggregate<TIdentity, TAggregate>;

        /// <summary>
        /// Parses the aggregate id from the stream name specified by <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Name of stream to parse.</param>
        /// <param name="configuration">Configuration of the aggregate.</param>
        /// <typeparam name="TIdentity">Type of aggregate ID</typeparam>
        /// <typeparam name="TAggregate">Type of aggregate</typeparam>
        /// <returns>The aggregate ID part of the stream name.</returns>
        TIdentity ParseId<TIdentity, TAggregate>(
            string stream,
            AggregateConfiguration<TIdentity, TAggregate> configuration
        )
            where TAggregate : Aggregate<TIdentity, TAggregate>;
    }
}