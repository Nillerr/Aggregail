using Newtonsoft.Json;

namespace EventSourcing.Demo.Cases.CRM
{
    public sealed class Incident
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("c2rur_urcasenumber")]
        public CaseNumber CaseNumber { get; set; }
        
        [JsonProperty("c2rur_casetype")]
        public CaseType Type { get; set; }
        
        [JsonProperty("caseorigincode")]
        public CaseOriginCode Origin { get; set; }
        
        [JsonProperty("statuscode")]
        public CaseStatus Status { get; set; }
        
        // Robot Stuff
        [JsonProperty("_c2rur_serialno_value")]
        public RobotId? RobotId { get; set; }
        
        [JsonProperty("aka_application_test")]
        public RobotApplication? Application { get; set; }
        
        [JsonProperty("_ur_new_softwareversion_value")]
        public SoftwareVersionId? SoftwareVersionId { get; set; }
        
        [JsonProperty("ur_errorcodes")]
        public RobotErrorCode[] ErrorCodes { get; set; }
    }
}