using System;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public readonly struct RobotDistributionId : IValueObject<Guid>, IEquatable<RobotDistributionId>
    {
        public Guid Value { get; }

        public RobotDistributionId(Guid value) => Value = value;

        public bool Equals(RobotDistributionId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is RobotDistributionId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(RobotDistributionId left, RobotDistributionId right) => left.Equals(right);
        public static bool operator !=(RobotDistributionId left, RobotDistributionId right) => !left.Equals(right);

        public override string ToString() => Value.ToString();
    }
}