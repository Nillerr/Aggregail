namespace Aggregail
{
    /// <summary>
    /// Contains additional information about a recorded event.
    /// </summary>
    /// <remarks>
    /// Used when replaying events to construct an Aggregate.
    /// </remarks>
    public readonly struct RecordableEvent
    {
        /// <summary>
        /// Creates an instance of the <see cref="RecordableEvent"/> struct.
        /// </summary>
        /// <param name="eventNumber">The Event Number associated with this event.</param>
        public RecordableEvent(long eventNumber)
        {
            EventNumber = eventNumber;
        }

        /// <summary>
        /// The Event Number associated with the recorded event.
        /// </summary>
        public long EventNumber { get; }
    }
}