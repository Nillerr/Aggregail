using EventSourcing.Demo.Cases.Events;

namespace EventSourcing.Demo.Cases
{
    public sealed class CaseCommentAttachment
    {
        public CaseCommentAttachment(PortalCommentAnnotationImported e)
        {
            Id = e.Id;
        }

        public CaseCommentAttachmentId Id { get; }

        public void Apply(PortalCommentAnnotationImported e)
        {
            // Nothing
        }
    }
}