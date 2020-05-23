using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aggregail
{
    /// <summary>
    /// An extension upon <see cref="Aggregate{TIdentity,TAggregate}"/>, which adds additional 
    /// </summary>
    /// <typeparam name="TIdentity"></typeparam>
    /// <typeparam name="TAggregate"></typeparam>
    public abstract class AbstractAggregate<TIdentity, TAggregate> : Aggregate<TIdentity, TAggregate>
        where TAggregate : AbstractAggregate<TIdentity, TAggregate>
    {
        protected static AggregateConfiguration<TIdentity, TAggregate> Configuration { get; set; } = null!;
        
        private static AggregateConfiguration<TIdentity, TAggregate> GetConfiguration() =>
            Configuration ?? throw new InvalidOperationException(
                $"The static property {nameof(Configuration)} must be set by the subclass"
            );

        public static Task<TAggregate?> FromAsync(TIdentity id, IEventStore store) =>
            store.AggregateAsync(id, GetConfiguration());

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
        /// Commits any pending events previously appended with <see cref="Append{T}"/> to the
        /// <see cref="IEventStore"/> specified by <paramref name="store"/>, and clears the queue of pending events.
        /// </summary>
        /// <param name="store">The <see cref="IEventStore"/> to store events in.</param>
        /// <returns>A <see cref="Task{TResult}"/></returns>
        /// <exception cref="InvalidOperationException">
        /// If there are no pending events, previously appended with <see cref="Append{T}"/>, to commit.
        /// </exception>
        public Task CommitAsync(IEventStore store) => CommitAsync(store, GetConfiguration());
    }
}