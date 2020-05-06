using System;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
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