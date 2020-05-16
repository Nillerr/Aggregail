using System;
using Aggregail;
using EventSourcing.Demo.Robots.Serialization;
using Newtonsoft.Json;

namespace EventSourcing.Demo.Robots
{
    [JsonConverter(typeof(RobotRegistrationIdConverter))]
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