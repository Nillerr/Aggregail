using Newtonsoft.Json;

namespace EventSourcing.Demo.Cases.CRM
{
    public sealed class Robot
    {
        [JsonProperty("c2rur_robotsid")]
        public RobotId Id { get; set; }
        
        [JsonProperty("c2rur_name")]
        public SerialNumber SerialNumber { get; set; }
        
        [JsonProperty("aka_application_test")]
        public RobotApplication? Application { get; set; }
        
        [JsonProperty("_ur_new_softwareversion_value")]
        public SoftwareVersionId? SoftwareVersionId { get; set; }
    }
}