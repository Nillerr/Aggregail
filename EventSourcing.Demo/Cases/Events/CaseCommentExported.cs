using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases.Events
{
    public sealed class CaseCommentExported
    {
        public static readonly EventType<CaseCommentExported> EventType = "CaseCommentExported";

        public CaseCommentExported(CaseCommentId id)
        {
            Id = id;
        }

        public CaseCommentId Id { get; }
    }
}