using System;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public readonly struct CaseNumber : IValueObject<string>, IEquatable<CaseNumber>
    {
        public string Value { get; }

        public CaseNumber(string value) => Value = value;

        public bool Equals(CaseNumber other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is CaseNumber other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(CaseNumber left, CaseNumber right) => left.Equals(right);
        public static bool operator !=(CaseNumber left, CaseNumber right) => !left.Equals(right);

        public override string ToString() => Value;
    }
}