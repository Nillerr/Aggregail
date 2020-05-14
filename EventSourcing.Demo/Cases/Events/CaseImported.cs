using Aggregail;

namespace EventSourcing.Demo.Cases.Events
{
    public sealed class CaseImported
    {
        public static readonly EventType<CaseImported> EventType = "CaseImported";
        
        public string Subject { get; }
        public string Description { get; }

        public string CaseNumber { get; }

        public CaseStatus Status { get; }

        public CaseImported(string subject, string description, string caseNumber, CaseStatus status)
        {
            Subject = subject;
            Description = description;
            CaseNumber = caseNumber;
            Status = status;
        }
    }
}