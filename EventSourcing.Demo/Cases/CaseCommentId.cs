using System;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public readonly struct CaseCommentId : IValueObject<Guid>, IEquatable<CaseCommentId>, IEquatable<PortalCommentId>
    {
        public Guid Value { get; }

        public CaseCommentId(Guid value) => Value = value;

        public bool Equals(CaseCommentId other) => Value == other.Value;
        public bool Equals(PortalCommentId other) => Value == other.Value;

        public override bool Equals(object? obj)
        {
            if (obj is CaseCommentId cci && Equals(cci)) return true;
            if (obj is PortalCommentId pci && Equals(pci)) return true;
            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(CaseCommentId left, CaseCommentId right) => left.Equals(right);
        public static bool operator !=(CaseCommentId left, CaseCommentId right) => !left.Equals(right);

        public static bool operator ==(CaseCommentId left, PortalCommentId right) => left.Equals(right);
        public static bool operator !=(CaseCommentId left, PortalCommentId right) => !left.Equals(right);

        public override string ToString() => Value.ToString();
    }
}