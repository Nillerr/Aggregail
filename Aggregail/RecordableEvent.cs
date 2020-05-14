namespace Aggregail
{
    public readonly struct RecordableEvent
    {
        public RecordableEvent(long eventNumber)
        {
            EventNumber = eventNumber;
        }

        public long EventNumber { get; }
    }
}