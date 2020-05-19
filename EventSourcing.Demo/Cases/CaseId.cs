using System;
using Aggregail;
using Newtonsoft.Json;

namespace EventSourcing.Demo.Cases
{
    [JsonConverter(typeof(CaseIdConverter))]
    public readonly struct CaseId : IValueObject<Guid>, IEquatable<CaseId>
    {
        public Guid Value { get; }

        public CaseId(Guid value) => Value = value;

        public bool Equals(CaseId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is CaseId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(CaseId left, CaseId right) => left.Equals(right);
        public static bool operator !=(CaseId left, CaseId right) => !left.Equals(right);

        public override string ToString() => Value.ToString();

        public static CaseId Parse(string input) => new CaseId(Guid.Parse(input));
    }
}