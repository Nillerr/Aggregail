using System;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public readonly struct CaseCommentAttachmentId : IValueObject<Guid>, IEquatable<CaseCommentAttachmentId>
    {
        public Guid Value { get; }

        public CaseCommentAttachmentId(Guid value) => Value = value;

        public bool Equals(CaseCommentAttachmentId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is CaseCommentAttachmentId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(CaseCommentAttachmentId left, CaseCommentAttachmentId right) => left.Equals(right);
        public static bool operator !=(CaseCommentAttachmentId left, CaseCommentAttachmentId right) => !left.Equals(right);

        public static implicit operator CaseCommentAttachmentId(AnnotationId id) => new CaseCommentAttachmentId(id.Value);

        public override string ToString() => Value.ToString();
    }
}