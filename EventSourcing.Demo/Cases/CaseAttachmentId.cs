using System;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public readonly struct CaseAttachmentId : IValueObject<Guid>, IEquatable<CaseAttachmentId>
    {
        public Guid Value { get; }

        public CaseAttachmentId(Guid value) => Value = value;

        public bool Equals(CaseAttachmentId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is CaseAttachmentId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(CaseAttachmentId left, CaseAttachmentId right) => left.Equals(right);
        public static bool operator !=(CaseAttachmentId left, CaseAttachmentId right) => !left.Equals(right);

        public static implicit operator CaseAttachmentId(AnnotationId id) => new CaseAttachmentId(id.Value);

        public override string ToString() => Value.ToString();
    }
}