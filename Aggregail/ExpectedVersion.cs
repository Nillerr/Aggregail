namespace Aggregail
{
    /// <summary>
    /// Special constants for designating the expected version of a stream.
    /// </summary>
    public static class ExpectedVersion
    {
        /// <summary>
        /// Designates a stream either does not exist, or exists but has no events yet.
        /// </summary>
        public const long NoStream = -1;

        /// <summary>
        /// The write should not conflict with anything and should always succeed.
        /// </summary>
        public const long Any = -2;
        
        /// <summary>
        /// The stream should exist. If it or a metadata stream does not exist treat that as a concurrency problem.
        /// </summary>
        public const long StreamExists = -4;
    }
}