using System;
using EventSourcing.Demo.Framework.Serialiazation;

namespace EventSourcing.Demo.Framework
{
    public sealed class PendingEvent<T> : IPendingEvent
    {
        public Guid Id { get; }
        public string Type { get; }
        
        public T Event { get; }

        public PendingEvent(Guid id, EventType<T> type, T @event)
        {
            Id = id;
            Type = type.Value;
            Event = @event;
        }

        public byte[] EncodedData(IJsonEncoder encoder) => encoder.Encode(Event);
    }
}