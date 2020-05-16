using System;
using Aggregail;
using EventSourcing.Demo.Robots.Serialization;
using Newtonsoft.Json;

namespace EventSourcing.Demo.Robots
{
    [JsonConverter(typeof(RobotIdConverter))]
    public readonly struct RobotId : IValueObject<Guid>, IEquatable<RobotId>
    {
        public Guid Value { get; }

        public RobotId(Guid value) => Value = value;

        public bool Equals(RobotId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is RobotId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(RobotId left, RobotId right) => left.Equals(right);
        public static bool operator !=(RobotId left, RobotId right) => !left.Equals(right);

        public override string ToString() => Value.ToString();
    }
}