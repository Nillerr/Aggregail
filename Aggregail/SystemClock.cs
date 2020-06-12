using System;

namespace Aggregail
{
    /// <summary>
    /// Singleton implementation of <see cref="IClock"/> which returns the current system time.
    /// </summary>
    public sealed class SystemClock : IClock
    {
        /// <summary>
        /// The singleton instance of <see cref="IClock"/>.
        /// </summary>
        public static readonly SystemClock Instance = new SystemClock();

        private SystemClock()
        {
        }

        /// <summary>
        /// The current system time as a <see cref="DateTime"/> in UTC.
        /// </summary>
        public DateTime UtcNow => DateTime.UtcNow;
    }
}