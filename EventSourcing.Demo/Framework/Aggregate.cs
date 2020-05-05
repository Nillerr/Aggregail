using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using JetBrains.Annotations;

namespace EventSourcing.Demo.Framework
{
    public abstract class Aggregate
    {
        internal Aggregate(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }

    public abstract class Aggregate<TAggregate, TCreateEvent> : Aggregate
        where TAggregate : Aggregate<TAggregate, TCreateEvent>
    {
        private readonly List<IPendingEvent> _pendingEvents = new List<IPendingEvent>();

        private long _versionNumber = ExpectedVersion.NoStream;

        protected Aggregate(Guid id, [PublicAPI("Serves as a reference")] TCreateEvent @event)
            : base(id)
        {
        }

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
            AggregateConfiguration<TAggregate, TCreateEvent> configuration
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