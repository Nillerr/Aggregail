using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace EventSourcing.Demo.Framework
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
        {
            var pendingEvent = new PendingEvent<T>(id, type, data);
            _pendingEvents.Add(pendingEvent);
        }

        public void Record(RecordableEvent recordableEvent)
        {
            var nextVersionNumber = _versionNumber + 1;
            if (recordableEvent.EventNumber != nextVersionNumber)
            {
                throw new InvalidOperationException();
            }

            _versionNumber = nextVersionNumber;
        }

        protected async Task CommitAsync(
            IEventStoreAppender appender,
            AggregateConfiguration<TIdentity, TAggregate> configuration
        )
        {
            if (_pendingEvents.Count == 0)
            {
                throw new InvalidOperationException();
            }

            await appender.AppendToStreamAsync(Id, configuration, _versionNumber, _pendingEvents);
        }
    }
}