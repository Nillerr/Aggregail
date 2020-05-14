using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Aggregail.MongoDB
{
    public sealed class RecordedEvent
    {
        [BsonElement("stream")]
        public string Stream { get; set; }
        
        [BsonElement("event_id")]
        public Guid EventId { get; set; }
        
        [BsonElement("event_type")]
        public string EventType { get; set; }
        
        [BsonElement("event_number")]
        public long EventNumber { get; set; }
        
        [BsonElement("data")]
        [BsonSerializer(typeof(JsonBsonDocumentSerializer))]
        public byte[] Data { get; set; }
    }
}