using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aggregail
{
    /// <summary>
    /// Base class of an Aggregate Root
    /// </summary>
    /// <typeparam name="TIdentity">Type of ID for the Aggregate</typeparam>
    /// <typeparam name="TAggregate">Type of Aggregate, referring to the subclass itself.</typeparam>
    /// <remarks>
    /// Intended to be subclassed like so:
    /// <code>
    /// public sealed class Goat : Aggregate&lt;GoatId, Goat&gt;
    /// {
    /// }
    /// </code>
    /// </remarks>
    public abstract class Aggregate<TIdentity, TAggregate>
        where TAggregate : Aggregate<TIdentity, TAggregate>
    {
        private readonly List<IPendingEvent> _pendingEvents = new List<IPendingEvent>();

        private long _versionNumber = ExpectedVersion.NoStream;

        /// <summary>
        /// Creates an instance of the aggregate.
        /// </summary>
        /// <param name="id">Id of the aggregate</param>
        protected Aggregate(TIdentity id)
        {
            Id = id;
        }

        /// <summary>
        /// Id of the aggregate
        /// </summary>
        public TIdentity Id { get; }

        /// <summary>
        /// Appends an event to the aggregate, so it can be committed in a later call to <see cref="CommitAsync"/>.
        /// </summary>
        /// <param name="id">Id of the event</param>
        /// <param name="type">Type of the event</param>
        /// <param name="data">Data of the event</param>
        /// <typeparam name="T">Type of the event</typeparam>
        protected void Append<T>(Guid id, EventType<T> type, T data)
            where T : class
        {
            var pendingEvent = new PendingEvent<T>(id, type, data);
            _pendingEvents.Add(pendingEvent);
        }

        /// <summary>
        /// Records additional information about a recorded event from the event store stream.
        /// </summary>
        /// <param name="recordableEvent">The recorded event information</param>
        /// <exception cref="WrongExpectedVersionException">
        /// The version of the recorded event is not the next expected event in the sequence.
        /// </exception>
        public void Record(RecordableEvent recordableEvent)
        {
            var expectedVersion = _versionNumber + 1;
            var eventNumber = recordableEvent.EventNumber;
            if (eventNumber != expectedVersion)
            {
                throw new WrongExpectedVersionException(
                    $"Expected a recorded event number {expectedVersion}, but was {eventNumber}", expectedVersion,
                    eventNumber
                );
            }

            _versionNumber = expectedVersion;
        }

        /// <summary>
        /// Commits any pending events previously appended with <see cref="Append{T}"/> to the
        /// <see cref="IEventStore"/> specified by <paramref name="store"/>, and clears the queue of pending events.
        /// </summary>
        /// <param name="store">The <see cref="IEventStore"/> to store events in.</param>
        /// <param name="configuration">The configuration of the aggregate.</param>
        /// <returns>A <see cref="Task{TResult}"/></returns>
        /// <exception cref="InvalidOperationException">
        /// If there are no pending events, previously appended with <see cref="Append{T}"/>, to commit.
        /// </exception>
        /// <remarks>
        /// Intended to be wrapped by a public member on the aggregate, in order to keep the aggregate configuration
        /// private, like so:
        /// <code>
        /// public sealed class Goat : Aggregate&lt;GoatId, Goat&gt;
        /// {
        ///     private static readonly AggregateConfiguration&lt;GoatId, Goat&gt; Configuration =
        ///         new AggregateConfiguration&lt;GoatId, Goat&gt;("goat");
        ///     
        ///     public Task CommitAsync(IEventStore store) => CommitAsync(store, Configuration);
        /// }
        /// </code>
        /// </remarks>
        protected async Task CommitAsync(IEventStore store, AggregateConfiguration<TIdentity, TAggregate> configuration)
        {
            if (_pendingEvents.Count == 0)
            {
                throw new InvalidOperationException("There are no pending events to commit");
            }

            await store.AppendToStreamAsync(Id, configuration, _versionNumber, _pendingEvents);
            _versionNumber += _pendingEvents.Count;
            _pendingEvents.Clear();
        }
    }
}