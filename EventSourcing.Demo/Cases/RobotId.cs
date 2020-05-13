using System;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public readonly struct RobotId : IValueObject<Guid>, IEquatable<RobotId>
    {
        public Guid Value { get; }

        public RobotId(Guid value) => Value = value;
        
        public static RobotId Create(Guid value) => new RobotId(value);

        public bool Equals(RobotId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is RobotId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(RobotId left, RobotId right) => left.Equals(right);
        public static bool operator !=(RobotId left, RobotId right) => !left.Equals(right);

        public override string ToString() => Value.ToString();
    }
}