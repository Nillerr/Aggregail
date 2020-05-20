using System;
using Newtonsoft.Json.Linq;

namespace Aggregail.MongoDB.Admin.Hubs
{
    public sealed class RecordedEvent
    {
        public string Id { get; set; }
        public string Stream { get; set; }
        public Guid EventId { get; set; }
        public string EventType { get; set; }
        public long EventNumber { get; set; }
        public DateTime Created { get; set; }
        public JObject Data { get; set; }
    }
}