using System;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public readonly struct SoftwareVersionId : IValueObject<Guid>, IEquatable<SoftwareVersionId>
    {
        public Guid Value { get; }

        public SoftwareVersionId(Guid value) => Value = value;

        public static SoftwareVersionId Create(Guid value) => new SoftwareVersionId(value);

        public bool Equals(SoftwareVersionId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is SoftwareVersionId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(SoftwareVersionId left, SoftwareVersionId right) => left.Equals(right);
        public static bool operator !=(SoftwareVersionId left, SoftwareVersionId right) => !left.Equals(right);

        public override string ToString() => Value.ToString();
    }
}