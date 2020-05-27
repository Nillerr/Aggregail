using System;
using System.Text.Json.Serialization;
using Aggregail.MongoDB.Admin.Documents;

namespace Aggregail.MongoDB.Admin.Hubs
{
    public sealed class RecordedEvent
    {
        public string Id { get; }
        public string Stream { get; }
        public Guid EventId { get; }
        public string EventType { get; }
        public long EventNumber { get; }
        public DateTime Created { get; }

        [JsonConverter(typeof(RawJsonConverter))]
        public byte[] Data { get; }
        
        [JsonConverter(typeof(RawJsonConverter))]
        public byte[]? Metadata { get; }

        public RecordedEvent(
            string id,
            string stream,
            Guid eventId,
            string eventType,
            long eventNumber,
            DateTime created,
            byte[] data,
            byte[]? metadata
        )
        {
            Id = id;
            Stream = stream;
            EventId = eventId;
            EventType = eventType;
            EventNumber = eventNumber;
            Created = created;
            Data = data;
            Metadata = metadata;
        }

        public static RecordedEvent FromDocument(RecordedEventDocument document)
        {
            var recordedEvent = new RecordedEvent(
                document.Id,
                document.Stream,
                document.EventId,
                document.EventType,
                document.EventNumber,
                document.Created,
                document.Data,
                document.Metadata
            );

            return recordedEvent;
        }
    }
}