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
    }
}