using System;

namespace Aggregail
{
    public sealed class PendingEvent<TData> : IPendingEvent
        where TData : class
    {
        private readonly TData _data;
        private readonly EventType<TData> _type;
        
        public Guid Id { get; }
        public string Type => _type.Value;

        public PendingEvent(Guid id, EventType<TData> type, TData data)
        {
            Id = id;
            _type = type.Value;
            _data = data;
        }

        public byte[] Data(IJsonEventSerializer serializer) => serializer.Serialize(_data);
    }
}