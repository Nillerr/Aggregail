using System;
using EventSourcing.Demo.Framework.Serialiazation;

namespace EventSourcing.Demo.Framework
{
    public sealed class PendingEvent<TData> : IPendingEvent
    {
        public Guid Id { get; }
        public string Type { get; }
        
        public TData Data { get; }

        public PendingEvent(Guid id, EventType<TData> type, TData data)
        {
            Id = id;
            Type = type.Value;
            Data = data;
        }

        public byte[] EncodedData(IJsonEncoder encoder) => encoder.Encode(Data);
    }
}