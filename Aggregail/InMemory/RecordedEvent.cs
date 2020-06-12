using System;

namespace Aggregail.InMemory
{
    internal sealed class RecordedEvent<TIdentity, TAggregate> : IRecordedEvent<TIdentity, TAggregate>
        where TAggregate : Aggregate<TIdentity, TAggregate>
    {
        private readonly StoredEvent _event;
        private readonly IJsonEventSerializer _serializer;

        public RecordedEvent(StoredEvent @event, IJsonEventSerializer serializer, TIdentity id)
        {
            _event = @event;
            _serializer = serializer;
            Id = id;
        }

        public string Stream => _event.EventStreamId;
        public Guid EventId => _event.EventId;
        public long EventNumber => _event.EventNumber;
        public string EventType => _event.EventType;
        public DateTime Created => _event.Created;

        public T Data<T>() where T : class => _serializer.Deserialize<T>(_event.Data);

        public T Metadata<T>() where T : class => _event.Metadata == null
            ? null!
            : _serializer.Deserialize<T>(_event.Metadata);
        
        public TIdentity Id { get; }
    }
}