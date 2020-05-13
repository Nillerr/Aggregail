namespace EventSourcing.Demo.Cases
{
    public sealed class CaseAttachment
    {
        public CaseAttachment(CaseAttachmentId id)
        {
            Id = id;
        }

        public CaseAttachmentId Id { get; }
    }
}