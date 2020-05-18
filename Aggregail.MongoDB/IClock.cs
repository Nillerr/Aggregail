using System;

namespace Aggregail.MongoDB
{
    /// <summary>
    /// A clock which returns the current time as a <see cref="DateTime"/> in UTC.
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// The current time as a <see cref="DateTime"/> in UTC, according to this clock.
        /// </summary>
        DateTime UtcNow { get; }
    }
}