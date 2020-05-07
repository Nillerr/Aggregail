using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases.Events
{
    public sealed class PortalCommentAnnotationImported
    {
        public static readonly EventType<PortalCommentAnnotationImported> EventType = "PortalCommentAnnotationImported";

        public PortalCommentAnnotationImported(AnnotationId id, PortalCommentId portalCommentId)
        {
            Id = id;
            PortalCommentId = portalCommentId;
        }

        public AnnotationId Id { get; }
        public PortalCommentId PortalCommentId { get; }
    }
}