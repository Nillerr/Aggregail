using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases.Events
{
    public sealed class CaseAssignedToService
    {
        public static EventType<CaseAssignedToService> EventType = "CaseAssignedToService";
    }
}