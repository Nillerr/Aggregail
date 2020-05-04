using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EventSourcing.Demo.Cases
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CaseStatus
    {
        InProgress,
        WaitingForDistributor,
        Resolved
    }
}