using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventSourcing.Demo.Framework.Serialiazation;
using EventStore.ClientAPI;

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
        private readonly Queue<IPendingEvent> _pendingEvents = new Queue<IPendingEvent>();

        private long _versionNumber = ExpectedVersion.NoStream;

        protected Aggregate(Guid id, TCreateEvent @event)
            : base(id)
        {
        }

        protected void Append<T>(Guid id, EventType<T> type, T data)
        {
            var pendingEvent = new PendingEvent<T>(id, type, data);
            _pendingEvents.Enqueue(pendingEvent);
        }

        public void Record(RecordedEvent recordedEvent)
        {
            var nextVersionNumber = _versionNumber + 1;
            if (recordedEvent.EventNumber != nextVersionNumber)
            {
                throw new InvalidOperationException();
            }

            _versionNumber = recordedEvent.EventNumber;
        }

        public async Task CommitAsync(
            IEventStoreConnection connection,
            AggregateConfiguration<TAggregate, TCreateEvent> configuration,
            IJsonEncoder encoder
        )
        {
            if (_pendingEvents.Count == 0)
            {
                throw new InvalidOperationException();
            }

            var events = _pendingEvents.Select(pendingEvent =>
                new EventData(pendingEvent.Id, pendingEvent.Type, true, pendingEvent.EncodedData(encoder), null)
            );

            var stream = configuration.Name.Stream(Id);
            await connection.AppendToStreamAsync(stream, _versionNumber, events);
        }
    }
}