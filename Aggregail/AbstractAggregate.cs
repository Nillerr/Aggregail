using System;
using System.Collections.Generic;
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
        /// <param name="id">Id of the aggregate.</param>
        /// <param name="store">Event store to query.</param>
        /// <returns>
        /// The constructed aggregate, or <c>null</c> if the stream does not exist in <paramref name="store"/>.
        /// </returns>
        public static Task<TAggregate?> FromAsync(TIdentity id, IEventStore store) =>
            store.AggregateAsync(id, GetConfiguration());

        /// <summary>
        /// Resolves the ids of all aggregates of this type, stored in <paramref name="store"/>.
        /// </summary>
        /// <param name="store">Event store to query.</param>
        /// <returns>The ids of all aggregates of this type.</returns>
        public static IAsyncEnumerable<TIdentity> IdsAsync(IEventStore store) =>
            store.AggregateIdsAsync(GetConfiguration());

        protected static TAggregate Create<T>(
            EventType<T> type,
            T data,
            Func<Guid, T, TAggregate> constructor
        )
            where T : class
        {
            var eventId = Guid.NewGuid();
            var aggregate = constructor(eventId, data);
            aggregate.Append(eventId, type, data);
            return aggregate;
        }

        protected static TAggregate Create<T>(
            EventType<T> type,
            T data,
            Func<T, TAggregate> constructor
        )
            where T : class
        {
            var eventId = Guid.NewGuid();
            var aggregate = constructor(data);
            aggregate.Append(eventId, type, data);
            return aggregate;
        }

        protected static TAggregate Create<T>(
            Guid eventId,
            EventType<T> type,
            T data,
            Func<T, TAggregate> constructor
        )
            where T : class
        {
            var aggregate = constructor(data);
            aggregate.Append(eventId, type, data);
            return aggregate;
        }

        protected static TAggregate Create<T>(
            Guid eventId,
            EventType<T> type,
            T data,
            Func<Guid, T, TAggregate> constructor
        )
            where T : class
        {
            var aggregate = constructor(eventId, data);
            aggregate.Append(eventId, type, data);
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
        /// <param name="applicator">Applicator of the event</param>
        /// <typeparam name="T">Type of the event</typeparam>
        protected void Append<T>(EventType<T> type, T data, Action<T> applicator)
            where T : class
        {
            Append(type, data);
            applicator(data);
        }

        /// <summary>
        /// Commits any pending events previously appended with one of the <c>Append</c> methods, to the
        /// <see cref="IEventStore"/> specified by <paramref name="store"/>, and clears the queue of pending events.
        /// </summary>
        /// <param name="store">The <see cref="IEventStore"/> to store events in.</param>
        /// <returns>A <see cref="Task{TResult}"/></returns>
        /// <exception cref="InvalidOperationException">
        /// If there are no pending events, previously appended with <c>Append</c>, to commit.
        /// </exception>
        public Task CommitAsync(IEventStore store) => CommitAsync(store, GetConfiguration());
    }
}