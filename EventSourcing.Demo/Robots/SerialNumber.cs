using System;
using Aggregail;
using EventSourcing.Demo.Robots.Serialization;
using Newtonsoft.Json;

namespace EventSourcing.Demo.Robots
{
    [JsonConverter(typeof(SerialNumberConverter))]
    public readonly struct SerialNumber : IValueObject<string>, IEquatable<SerialNumber>
    {
        public string Value { get; }

        public SerialNumber(string value) => Value = value;

        public bool Equals(SerialNumber other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is SerialNumber other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(SerialNumber left, SerialNumber right) => left.Equals(right);
        public static bool operator !=(SerialNumber left, SerialNumber right) => !left.Equals(right);

        public override string ToString() => Value.ToString();
    }
}