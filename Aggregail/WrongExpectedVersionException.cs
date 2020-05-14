using System;

namespace Aggregail
{
    public sealed class WrongExpectedVersionException : Exception
    {
        public long? ExpectedVersion { get; }
        public long? ActualVersion { get; }

        public WrongExpectedVersionException(string message, long? expectedVersion, long? actualVersion)
            : base(message)
        {
            ExpectedVersion = expectedVersion;
            ActualVersion = actualVersion;
        }
    }
}