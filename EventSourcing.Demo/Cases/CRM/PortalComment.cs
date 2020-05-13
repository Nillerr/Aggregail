using Newtonsoft.Json;

namespace EventSourcing.Demo.Cases.CRM
{
    public sealed class PortalComment : Activity
    {
        [JsonProperty("adx_portalcommentdirectioncode")]
        public PortalCommentDirectionCode? DirectionCode { get; }
        
        [JsonProperty("ur_audience")]
        public PortalCommentAudience Audience { get; }
    }
}