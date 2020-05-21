using System;

namespace Aggregail.MongoDB.Admin.Controllers
{
    public sealed class Stream : IEquatable<Stream>
    {
        public Stream(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public bool Equals(Stream? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is Stream other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(Stream? left, Stream? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Stream? left, Stream? right)
        {
            return !Equals(left, right);
        }
    }
}