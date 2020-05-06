using System;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public readonly struct UserId : IValueObject<Guid>, IEquatable<UserId>
    {
        public Guid Value { get; }

        public UserId(Guid value) => Value = value;

        public bool Equals(UserId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is UserId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(UserId left, UserId right) => left.Equals(right);
        public static bool operator !=(UserId left, UserId right) => !left.Equals(right);

        public override string ToString() => Value.ToString();
    }
}