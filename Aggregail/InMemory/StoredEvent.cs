using System;

namespace Aggregail.InMemory
{
    internal sealed class StoredEvent
    {
        public StoredEvent(
            Guid eventId,
            string eventStreamId,
            string eventType,
            long eventNumber,
            byte[] data,
            byte[]? metadata,
            DateTime created
        )
        {
            EventId = eventId;
            EventStreamId = eventStreamId;
            EventType = eventType;
            EventNumber = eventNumber;
            Data = data;
            Metadata = metadata;
            Created = created;
        }

        public Guid EventId { get; }
        public string EventStreamId { get; }
        public string EventType { get; }
        public long EventNumber { get; }
        public byte[] Data { get; }
        public byte[]? Metadata { get; }
        public DateTime Created { get; }
    }
}