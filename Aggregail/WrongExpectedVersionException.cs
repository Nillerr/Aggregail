using System;

namespace Aggregail
{
    /// <summary>
    /// A version mismatch occured, where the actual version did not match the expected version.
    /// </summary>
    public sealed class WrongExpectedVersionException : Exception
    {
        /// <summary>
        /// The expected stream version.
        /// </summary>
        public long? ExpectedVersion { get; }
        
        /// <summary>
        /// The actual stream version.
        /// </summary>
        public long? ActualVersion { get; }

        /// <summary>
        /// Creates an instance of the <see cref="WrongExpectedVersionException"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="expectedVersion"></param>
        /// <param name="actualVersion"></param>
        public WrongExpectedVersionException(string message, long? expectedVersion, long? actualVersion)
            : base(message)
        {
            ExpectedVersion = expectedVersion;
            ActualVersion = actualVersion;
        }
    }
}