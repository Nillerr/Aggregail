using System;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public readonly struct AnnotationId : IValueObject<Guid>, IEquatable<AnnotationId>
    {
        public Guid Value { get; }

        public AnnotationId(Guid value) => Value = value;

        public bool Equals(AnnotationId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is AnnotationId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(AnnotationId left, AnnotationId right) => left.Equals(right);
        public static bool operator !=(AnnotationId left, AnnotationId right) => !left.Equals(right);

        public override string ToString() => Value.ToString();
    }
}