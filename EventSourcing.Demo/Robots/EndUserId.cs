using System;
using Aggregail;
using EventSourcing.Demo.Robots.Serialization;
using Newtonsoft.Json;

namespace EventSourcing.Demo.Robots
{
    [JsonConverter(typeof(EndUserIdConverter))]
    public readonly struct EndUserId : IValueObject<Guid>, IEquatable<EndUserId>
    {
        public Guid Value { get; }

        public EndUserId(Guid value) => Value = value;

        public bool Equals(EndUserId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is EndUserId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(EndUserId left, EndUserId right) => left.Equals(right);
        public static bool operator !=(EndUserId left, EndUserId right) => !left.Equals(right);

        public override string ToString() => Value.ToString();
    }
}