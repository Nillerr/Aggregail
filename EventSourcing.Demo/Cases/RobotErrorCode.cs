using System;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public readonly struct RobotErrorCode : IValueObject<string>, IEquatable<RobotErrorCode>
    {
        public string Value { get; }

        public RobotErrorCode(string value) => Value = value;

        public bool Equals(RobotErrorCode other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is RobotErrorCode other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(RobotErrorCode left, RobotErrorCode right) => left.Equals(right);
        public static bool operator !=(RobotErrorCode left, RobotErrorCode right) => !left.Equals(right);

        public override string ToString() => Value.ToString();
    }
}