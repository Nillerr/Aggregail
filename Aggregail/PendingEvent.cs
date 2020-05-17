using System;

namespace Aggregail
{
    /// <inheritdoc/>
    /// <typeparam name="TData">Type of event data</typeparam>
    public sealed class PendingEvent<TData> : IPendingEvent
        where TData : class
    {
        private readonly TData _data;
        private readonly EventType<TData> _type;

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public string Type => _type.Value;

        /// <summary>
        /// Creates an instance of the <see cref="PendingEvent{TData}"/> class.
        /// </summary>
        /// <param name="id">Id of the event</param>
        /// <param name="type">Type of the event, matching <paramref name="data"/>.</param>
        /// <param name="data">Object of the event data</param>
        public PendingEvent(Guid id, EventType<TData> type, TData data)
        {
            Id = id;
            _type = type.Value;
            _data = data;
        }

        /// <inheritdoc />
        public byte[] Data(IJsonEventSerializer serializer) => serializer.Serialize(_data);
    }
}