using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Aggregail
{
    /// <summary>
    /// Entry point to the underlying Event Store.
    /// </summary>
    /// <remarks>
    /// Consumers of the framework need not to concern themselves with this type, as it is merely a storage mechanism
    /// used by the framework. Implement this interface to access an Event Store database, which is not already
    /// supported by the framework.
    /// </remarks>
    public interface IEventStore
    {
        /// <summary>
        /// Appends pending events from an aggregate asynchronously to a stream.
        /// </summary>
        /// <param name="id">Aggregate id</param>
        /// <param name="configuration">Aggregate configuration</param>
        /// <param name="expectedVersion">Version the stream is expected to be at</param>
        /// <param name="pendingEvents">Events to store</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <typeparam name="TIdentity">Type of <paramref name="id"/></typeparam>
        /// <typeparam name="TAggregate">Type of aggregate</typeparam>
        /// <exception cref="WrongExpectedVersionException">The <paramref name="expectedVersion"/> did not match the current version of the stream.</exception>
        /// <returns>A <see cref="Task"/></returns>
        Task AppendToStreamAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long expectedVersion,
            IEnumerable<IPendingEvent> pendingEvents,
            CancellationToken cancellationToken = default
        )
            where TAggregate : Aggregate<TIdentity, TAggregate>;

        /// <summary>
        /// Asynchronously constructs an aggregate from an event stream, by replaying every event in the stream in
        /// order (oldest to newest), invoking the event applicators configured in <paramref name="configuration"/>.
        /// </summary>
        /// <param name="id">Aggregate id</param>
        /// <param name="configuration">Aggregate configuration</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <typeparam name="TIdentity">Type of <paramref name="id"/></typeparam>
        /// <typeparam name="TAggregate">Type of aggregate</typeparam>
        /// <returns>The constructed aggregate</returns>
        Task<TAggregate?> AggregateAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            CancellationToken cancellationToken = default
        )
            where TAggregate : Aggregate<TIdentity, TAggregate>;

        /// <summary>
        /// Returns the ids of every aggregate of the type <typeparamref name="TAggregate"/>, using the constructors
        /// declared in <paramref name="configuration"/>. 
        /// </summary>
        /// <param name="configuration">Aggregate configuration</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <typeparam name="TIdentity">Type of id</typeparam>
        /// <typeparam name="TAggregate">Type of aggregate</typeparam>
        /// <returns>The ids of every aggregate of type <typeparamref name="TAggregate"/>.</returns>
        IAsyncEnumerable<TIdentity> AggregateIdsAsync<TIdentity, TAggregate>(
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            [EnumeratorCancellation] CancellationToken cancellationToken = default
        )
            where TAggregate : Aggregate<TIdentity, TAggregate>;

        Task DeleteAggregateAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long expectedVersion,
            CancellationToken cancellationToken = default
        ) where TAggregate : Aggregate<TIdentity, TAggregate>;
    }
}