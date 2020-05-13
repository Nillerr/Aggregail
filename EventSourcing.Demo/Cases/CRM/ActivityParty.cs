using System;
using Newtonsoft.Json;

namespace EventSourcing.Demo.Cases.CRM
{
    public sealed class ActivityParty
    {
        [JsonProperty("_partyid_value")]
        public Guid PartyId { get; set; }
        
        [JsonProperty("participationtypemask")]
        public ParticipationTypeMask ParticipationTypeMask { get; set; }
    }
}