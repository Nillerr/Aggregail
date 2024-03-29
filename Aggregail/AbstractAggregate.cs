using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Aggregail
{
    /// <summary>
    /// An extension upon <see cref="Aggregate{TIdentity,TAggregate}"/>, which implements a lot of the boilerplate that
    /// would otherwise commonly be implemented.
    /// </summary>
    /// <typeparam name="TIdentity">Type of ID for the Aggregate</typeparam>
    /// <typeparam name="TAggregate">Type of Aggregate, referring to the subclass itself.</typeparam>
    /// <remarks>
    /// Intended to be subclassed like so:
    /// <code>
    /// public sealed class Goat : AbstractAggregate&lt;GoatId, Goat&gt;
    /// {
    ///     static Goat()
    ///     {
    ///         Configuration = new AggregateConfiguration&lt;GoatId, Goat&gt;("Goat", GoatId.Parse);
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public abstract class AbstractAggregate<TIdentity, TAggregate> : Aggregate<TIdentity, TAggregate>
        where TAggregate : AbstractAggregate<TIdentity, TAggregate>
    {
        static AbstractAggregate()
        {
            RuntimeHelpers.RunClassConstructor(typeof(TAggregate).TypeHandle);
        }

        /// <summary>
        /// The configuration of this aggregate
        /// </summary>
        protected static AggregateConfiguration<TIdentity, TAggregate> Configuration { get; set; } = null!;

        private static AggregateConfiguration<TIdentity, TAggregate> GetConfiguration() =>
            Configuration ?? throw new InvalidOperationException(
                $"The static property {nameof(Configuration)} must be set by the subclass"
            );

        /// <summary>
        /// Locates the aggregate stored in <paramref name="store"/>, identified by <paramref name="id"/>.
        /// </summary>
        /// <param name="store">Event store to query.</param>
        /// <param name="id">Id of the aggregate.</param>
        /// <param name="version">Version of the aggregate, or <c>null</c> for the latest version.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// The constructed aggregate, or <c>null</c> if the stream does not exist in <paramref name="store"/>.
        /// </returns>
        public static Task<TAggregate?> FromAsync(
            IEventStore store,
            TIdentity id,
            long? version = null,
            CancellationToken cancellationToken = default
        ) =>
            store.AggregateAsync(id, GetConfiguration(), version, cancellationToken);

        /// <summary>
        /// Resolves the ids of all aggregates of this type, stored in <paramref name="store"/>.
        /// </summary>
        /// <param name="store">Event store to query.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The ids of all aggregates of this type.</returns>
        public static IAsyncEnumerable<TIdentity> IdsAsync(
            IEventStore store,
            CancellationToken cancellationToken = default
        ) =>
            store.AggregateIdsAsync(GetConfiguration(), cancellationToken);

        /// <summary>
        /// Deletes the aggregate identified by <paramref name="id"/> from <paramref name="store"/>, regardless of
        /// which version the stream is currently at. 
        /// </summary>
        /// <param name="store">The <see cref="IEventStore"/> containing the aggregate stream.</param>
        /// <param name="id">The id of the aggregate to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}"/></returns>
        public static Task DeleteFromAsync(
            IEventStore store,
            TIdentity id,
            CancellationToken cancellationToken = default
        ) =>
            store.DeleteAggregateAsync(id, GetConfiguration(), ExpectedVersion.Any, cancellationToken);

        /// <summary>
        /// Determines if the aggregate specified by <paramref name="id"/> exists in <paramref name="store"/>.
        /// </summary>
        /// <param name="store">The <see cref="IEventStore"/></param>
        /// <param name="id">The id of the aggregate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><c>true</c> if the aggregate exists; <c>false</c> if it does not exist.</returns>
        public static Task<bool> ExistsAsync(
            IEventStore store,
            TIdentity id,
            CancellationToken cancellationToken = default
        ) =>
            store.AggregateExistsAsync(id, GetConfiguration(), cancellationToken);

        /// <summary>
        /// Returns every event for every aggregate of the this type.
        /// </summary>
        /// <param name="store">The store to return events from.</param>
        /// <param name="start">The offset to read events from</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A stream of events</returns>
        public static IAsyncEnumerable<IRecordedEvent<TIdentity, TAggregate>> RecordedEventsAsync(
            IEventStore store,
            long start,
            CancellationToken cancellationToken = default
        ) => store.ReadStreamEventsAsync(Configuration, start, cancellationToken);

        protected static TAggregate Create<TData>(
            EventType<TData> type,
            TData data,
            Func<Guid, TData, TAggregate> constructor
        )
            where TData : class
        {
            var eventId = Guid.NewGuid();
            var aggregate = constructor(eventId, data);
            aggregate.Append(eventId, type, data);
            return aggregate;
        }

        protected static TAggregate Create<TData, TMetadata>(
            EventType<TData> type,
            TData data,
            TMetadata metadata,
            Func<Guid, TData, TAggregate> constructor
        )
            where TData : class 
            where TMetadata : class
        {
            var eventId = Guid.NewGuid();
            var aggregate = constructor(eventId, data);
            aggregate.Append(eventId, type, data, metadata);
            return aggregate;
        }

        protected static TAggregate Create<TData>(
            Guid eventId,
            EventType<TData> type,
            TData data,
            Func<Guid, TData, TAggregate> constructor
        )
            where TData : class
        {
            var aggregate = constructor(eventId, data);
            aggregate.Append(eventId, type, data);
            return aggregate;
        }

        protected static TAggregate Create<TData, TMetadata>(
            Guid eventId,
            EventType<TData> type,
            TData data,
            TMetadata metadata,
            Func<Guid, TData, TAggregate> constructor
        )
            where TData : class
            where TMetadata : class
        {
            var aggregate = constructor(eventId, data);
            aggregate.Append(eventId, type, data, metadata);
            return aggregate;
        }

        protected AbstractAggregate(TIdentity id)
            : base(id)
        {
        }

        /// <summary>
        /// Appends an event to the aggregate, so it can be committed in a later call to <see cref="CommitAsync"/>.
        /// </summary>
        /// <param name="type">Type of the event</param>
        /// <param name="data">Data of the event</param>
        /// <typeparam name="T">Type of the event</typeparam>
        protected void Append<T>(EventType<T> type, T data)
            where T : class
        {
            Append(Guid.NewGuid(), type, data);
        }

        /// <summary>
        /// Appends an event to the aggregate, so it can be committed in a later call to <see cref="CommitAsync"/>.
        /// </summary>
        /// <param name="type">Type of the event</param>
        /// <param name="data">Data of the event</param>
        /// <param name="metadata">Metadata</param>
        /// <typeparam name="T">Type of the event</typeparam>
        /// <typeparam name="TMetadata">Type of metadata</typeparam>
        protected void Append<T, TMetadata>(EventType<T> type, T data, TMetadata metadata)
            where T : class
            where TMetadata : class
        {
            Append(Guid.NewGuid(), type, data, metadata);
        }

        /// <summary>
        /// Appends an event to the aggregate, so it can be committed in a later call to <see cref="CommitAsync"/>.
        /// </summary>
        /// <param name="id">Id of the event</param>
        /// <param name="type">Type of the event</param>
        /// <param name="data">Data of the event</param>
        /// <param name="applicator">Applicator of the event</param>
        /// <typeparam name="T">Type of the event</typeparam>
        protected void Append<T>(Guid id, EventType<T> type, T data, Action<T> applicator)
            where T : class
        {
            Append(id, type, data);
            applicator(data);
        }

        /// <summary>
        /// Appends an event to the aggregate, so it can be committed in a later call to <see cref="CommitAsync"/>.
        /// </summary>
        /// <param name="type">Type of the event</param>
        /// <param name="data">Data of the event</param>
        /// <param name="metadata">Metadata</param>
        /// <param name="applicator">Applicator of the event</param>
        /// <typeparam name="TData">Type of the event</typeparam>
        /// <typeparam name="TMetadata">Type of metadata</typeparam>
        protected void Append<TData, TMetadata>(Guid id, EventType<TData> type, TData data, TMetadata metadata, Action<TData> applicator)
            where TData : class
            where TMetadata : class
        {
            Append(id, type, data, metadata);
            applicator(data);
        }

        /// <summary>
        /// Appends an event to the aggregate, so it can be committed in a later call to <see cref="CommitAsync"/>.
        /// </summary>
        /// <param name="type">Type of the event</param>
        /// <param name="data">Data of the event</param>
        /// <param name="applicator">Applicator of the event</param>
        /// <typeparam name="T">Type of the event</typeparam>
        protected void Append<T>(EventType<T> type, T data, Action<T> applicator)
            where T : class
        {
            Append(type, data);
            applicator(data);
        }

        /// <summary>
        /// Appends an event to the aggregate, so it can be committed in a later call to <see cref="CommitAsync"/>.
        /// </summary>
        /// <param name="type">Type of the event</param>
        /// <param name="data">Data of the event</param>
        /// <param name="metadata">Metadata</param>
        /// <param name="applicator">Applicator of the event</param>
        /// <typeparam name="TData">Type of the event</typeparam>
        /// <typeparam name="TMetadata">Type of metadata</typeparam>
        protected void Append<TData, TMetadata>(EventType<TData> type, TData data, TMetadata metadata, Action<TData> applicator)
            where TData : class
            where TMetadata : class
        {
            Append(type, data, metadata);
            applicator(data);
        }

        /// <summary>
        /// Commits any pending events previously appended with one of the <c>Append</c> methods, to the
        /// <see cref="IEventStore"/> specified by <paramref name="store"/>, and clears the queue of pending events.
        /// </summary>
        /// <param name="store">The <see cref="IEventStore"/> to store events in.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}"/></returns>
        /// <exception cref="InvalidOperationException">
        /// If there are no pending events, previously appended with <c>Append</c>, to commit.
        /// </exception>
        public Task CommitAsync(IEventStore store, CancellationToken cancellationToken = default) =>
            CommitAsync(store, GetConfiguration(), cancellationToken);

        /// <summary>
        /// Deletes the aggregate from <paramref name="store"/>, while adhering to optimistic concurrency checks.
        /// </summary>
        /// <param name="store">The <see cref="IEventStore"/> containing the aggregate stream.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}"/></returns>
        public Task DeleteAsync(IEventStore store, CancellationToken cancellationToken = default) =>
            DeleteAsync(store, GetConfiguration(), cancellationToken);
    }
}