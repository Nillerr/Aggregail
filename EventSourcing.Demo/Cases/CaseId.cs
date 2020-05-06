using System;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public readonly struct CaseId : IValueObject<Guid>, IEquatable<CaseId>
    {
        public Guid Value { get; }

        public CaseId(Guid value) => Value = value;

        public bool Equals(CaseId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is CaseId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(CaseId left, CaseId right) => left.Equals(right);
        public static bool operator !=(CaseId left, CaseId right) => !left.Equals(right);

        public override string ToString() => Value.ToString();
    }
}