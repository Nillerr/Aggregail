using System;
using Newtonsoft.Json;
using NodaTime;

namespace EventSourcing.Demo.Cases.CRM
{
    public class Activity
    {
        [JsonProperty("activityid")]
        public Guid Id { get; set; }
        
        [JsonProperty("_regardingobjectid_value")]
        public Guid RegardingObjectId { get; set; }
        
        [JsonProperty("_ownerid_value")]
        public Guid OwnerId { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("createdon")]
        public Instant CreatedOn { get; set; }
        
        [JsonProperty("overriddencreatedon")]
        public Instant? OverriddenCreatedOn { get; set; }
    }
}