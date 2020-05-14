using Aggregail;

namespace EventSourcing.Demo.Cases.Events
{
    public sealed class CaseAssignedToDistributor
    {
        public static readonly EventType<CaseAssignedToDistributor> EventType = "CaseAssignedToDistributor";
    }
}