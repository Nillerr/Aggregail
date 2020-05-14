using Aggregail;

namespace EventSourcing.Demo.Cases.Events
{
    public sealed class CaseAssignedToService
    {
        public static readonly EventType<CaseAssignedToService> EventType = "CaseAssignedToService";
    }
}