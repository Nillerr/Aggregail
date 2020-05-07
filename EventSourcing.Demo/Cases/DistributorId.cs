using System;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public readonly struct DistributorId : IValueObject<Guid>, IEquatable<DistributorId>
    {
        public Guid Value { get; }

        public DistributorId(Guid value) => Value = value;

        public bool Equals(DistributorId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is DistributorId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(DistributorId left, DistributorId right) => left.Equals(right);
        public static bool operator !=(DistributorId left, DistributorId right) => !left.Equals(right);

        public override string ToString() => Value.ToString();
    }
}