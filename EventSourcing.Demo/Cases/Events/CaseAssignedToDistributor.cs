using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases.Events
{
    public sealed class CaseAssignedToDistributor
    {
        public static EventType<CaseAssignedToDistributor> EventType = "CaseAssignedToDistributor";
    }
}