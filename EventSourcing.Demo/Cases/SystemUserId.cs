using System;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public readonly struct SystemUserId : IValueObject<Guid>, IEquatable<SystemUserId>
    {
        public Guid Value { get; }

        public SystemUserId(Guid value) => Value = value;

        public bool Equals(SystemUserId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is SystemUserId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(SystemUserId left, SystemUserId right) => left.Equals(right);
        public static bool operator !=(SystemUserId left, SystemUserId right) => !left.Equals(right);

        public override string ToString() => Value.ToString();
    }
}