using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aggregail
{
    public interface IAggregateStore<TIdentity, TAggregate>
        where TAggregate : Aggregate<TIdentity, TAggregate>
    {
        /// <summary>
        /// Locates the aggregate identified by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Id of the aggregate.</param>
        /// <param name="version">Version of the aggregate, or <c>null</c> for the latest version.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// The constructed aggregate, or <c>null</c> if the stream does not exist.
        /// </returns>
        Task<TAggregate?> AggregateAsync(
            TIdentity id,
            long? version = null,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Resolves the ids of all aggregates of this type.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The ids of all aggregates of this type.</returns>
        IAsyncEnumerable<TIdentity> IdsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the aggregate identified by <paramref name="id"/>, regardless of
        /// which version the stream is currently at. 
        /// </summary>=
        /// <param name="id">The id of the aggregate to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <seealso cref="DeleteAsync(TAggregate,System.Threading.CancellationToken)"/>
        /// <returns>A <see cref="Task{TResult}"/></returns>
        Task DeleteAsync(TIdentity id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines if the aggregate specified by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the aggregate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><c>true</c> if the aggregate exists; <c>false</c> if it does not exist.</returns>
        Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns every event for every aggregate of the this type.
        /// </summary>
        /// <param name="start">The offset to read events from</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A stream of events</returns>
        IAsyncEnumerable<IRecordedEvent<TIdentity, TAggregate>> RecordedEventsAsync(
            long start,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Commits any pending events previously appended with one of the <c>Append</c> methods, and clears the queue
        /// of pending events.
        /// </summary>
        /// <param name="aggregate">The aggregate</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}"/></returns>
        /// <exception cref="InvalidOperationException">
        /// If there are no pending events, previously appended with <c>Append</c>, to commit.
        /// </exception>
        Task CommitAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the aggregate, while adhering to optimistic concurrency checks.
        /// </summary>
        /// <param name="aggregate">The aggregate</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}"/></returns>
        Task DeleteAsync(TAggregate aggregate, CancellationToken cancellationToken = default);
    }
}