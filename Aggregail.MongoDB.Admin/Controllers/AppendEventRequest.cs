using System;
using System.Text.Json.Serialization;
using Aggregail.MongoDB.Admin.Hubs;

namespace Aggregail.MongoDB.Admin.Controllers
{
    public sealed class AppendEventRequest
    {
        public Guid EventId { get; set; }
        public string EventType { get; set; } = null!;

        [JsonConverter(typeof(RawJsonConverter))]
        public byte[] Data { get; set; } = null!;
    }
}