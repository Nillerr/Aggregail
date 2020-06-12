using System;

namespace Aggregail.MongoDB
{
    internal sealed class RecordedEvent<TIdentity, TAggregate> : IRecordedEvent<TIdentity, TAggregate>
        where TAggregate : Aggregate<TIdentity, TAggregate>
    {
        private readonly RecordedEventDocument _document;
        private readonly IJsonEventSerializer _serializer;

        public RecordedEvent(RecordedEventDocument document, IJsonEventSerializer serializer, TIdentity id)
        {
            Id = id;
            _document = document;
            _serializer = serializer;
        }

        public string Stream => _document.Stream;
        public Guid EventId => _document.EventId;
        public long EventNumber => _document.EventNumber;
        public string EventType => _document.EventType;
        public DateTime Created => _document.Created;
        public T Data<T>() where T : class => _serializer.Deserialize<T>(_document.Data);

        public T Metadata<T>() where T : class => _document.Metadata == null
            ? null!
            : _serializer.Deserialize<T>(_document.Metadata);
        
        public TIdentity Id { get; }
    }
}