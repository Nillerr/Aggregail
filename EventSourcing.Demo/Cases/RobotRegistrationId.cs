using System;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public readonly struct RobotRegistrationId : IValueObject<Guid>, IEquatable<RobotRegistrationId>
    {
        public Guid Value { get; }

        public RobotRegistrationId(Guid value) => Value = value;

        public bool Equals(RobotRegistrationId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is RobotRegistrationId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(RobotRegistrationId left, RobotRegistrationId right) => left.Equals(right);
        public static bool operator !=(RobotRegistrationId left, RobotRegistrationId right) => !left.Equals(right);

        public override string ToString() => Value.ToString();
    }
}