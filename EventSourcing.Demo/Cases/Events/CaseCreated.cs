using System.Collections.Immutable;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases.Events
{
    public sealed class CaseCreated
    {
        public static EventType<CaseCreated> EventType = "CaseCreated";
        
        public string Subject { get; }
        public string Description { get; }
        
        public CaseType Type { get; }
        
        public ImmutableList<CaseAttachmentId> AttachmentIds { get; }

        public CaseCreated(string subject, string description, CaseType type, ImmutableList<CaseAttachmentId> attachmentIds)
        {
            Subject = subject;
            Description = description;
            Type = type;
            AttachmentIds = attachmentIds;
        }
    }
}