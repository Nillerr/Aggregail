using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aggregail
{
    public abstract class Aggregate<TIdentity, TAggregate>
        where TAggregate : Aggregate<TIdentity, TAggregate>
    {
        private readonly List<IPendingEvent> _pendingEvents = new List<IPendingEvent>();

        private long _versionNumber = ExpectedVersion.NoStream;

        protected Aggregate(TIdentity id)
        {
            Id = id;
        }

        public TIdentity Id { get; }

        protected void Append<T>(Guid id, EventType<T> type, T data)
            where T : class
        {
            var pendingEvent = new PendingEvent<T>(id, type, data);
            _pendingEvents.Add(pendingEvent);
        }

        public void Record(RecordableEvent recordableEvent)
        {
            var nextVersionNumber = _versionNumber + 1;
            if (recordableEvent.EventNumber != nextVersionNumber)
            {
                throw new InvalidOperationException("The versions are out of sync");
            }

            _versionNumber = nextVersionNumber;
        }

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