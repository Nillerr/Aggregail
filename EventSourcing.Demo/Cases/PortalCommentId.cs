using System;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public readonly struct PortalCommentId : IValueObject<Guid>, IEquatable<PortalCommentId>, IEquatable<CaseCommentId>
    {
        public Guid Value { get; }

        public PortalCommentId(Guid value) => Value = value;

        public bool Equals(PortalCommentId other) => Value == other.Value;
        public bool Equals(CaseCommentId other) => Value == other.Value;

        public override bool Equals(object? obj)
        {
            if (obj is PortalCommentId pci && Equals(pci)) return true;
            if (obj is CaseCommentId cci && Equals(cci)) return true;
            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(PortalCommentId left, PortalCommentId right) => left.Equals(right);
        public static bool operator !=(PortalCommentId left, PortalCommentId right) => !left.Equals(right);

        public static bool operator ==(PortalCommentId left, CaseCommentId right) => left.Equals(right);
        public static bool operator !=(PortalCommentId left, CaseCommentId right) => !left.Equals(right);

        public override string ToString() => Value.ToString();
    }
}