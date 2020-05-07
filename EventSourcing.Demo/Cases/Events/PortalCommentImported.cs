using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases.Events
{
    public sealed class PortalCommentImported
    {
        public static readonly EventType<PortalCommentImported> EventType = "PortalCommentImported";

        public PortalCommentImported(PortalCommentId id, string description, SystemUserId ownerId)
        {
            Id = id;
            Description = description;
            OwnerId = ownerId;
        }

        public PortalCommentId Id { get; }
        public string Description { get; }
        public SystemUserId OwnerId { get; }
    }
}