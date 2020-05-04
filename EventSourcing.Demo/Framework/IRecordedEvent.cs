namespace EventSourcing.Demo.Framework
{
    public readonly struct RecordedEvent
    {
        public RecordedEvent(long eventNumber)
        {
            EventNumber = eventNumber;
        }

        public long EventNumber { get; }
    }
}