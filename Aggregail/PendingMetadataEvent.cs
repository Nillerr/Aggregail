using System;

namespace Aggregail
{
    /// <inheritdoc/>
    /// <typeparam name="TData">Type of event data</typeparam>
    /// <typeparam name="TMetadata">Type of event metadata</typeparam>
    internal sealed class PendingMetadataEvent<TData, TMetadata> : IPendingEvent
        where TData : class
        where TMetadata : class
    {
        private readonly TData _data;
        private readonly EventType<TData> _type;
        
        private readonly TMetadata _metadata;

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public string Type => _type.Value;
        
        /// <summary>
        /// Creates an instance of the <see cref="PendingMetadataEvent{TData,TMetadata}"/> class.
        /// </summary>
        /// <param name="id">Id of the event</param>
        /// <param name="type">Type of the event, matching <paramref name="data"/>.</param>
        /// <param name="data">Object of the event data</param>
        /// <param name="metadata">Object of the event metadata</param>
        public PendingMetadataEvent(Guid id, EventType<TData> type, TData data, TMetadata metadata)
        {
            Id = id;
            _type = type.Value;
            _data = data;
            _metadata = metadata;
        }

        /// <inheritdoc />
        public byte[] Data(IJsonEventSerializer serializer) => 
            serializer.Serialize(_data);
        
        /// <inheritdoc />
        public byte[]? Metadata(IJsonEventSerializer serializer) => 
            _metadata == null ? null : serializer.Serialize(_metadata);
    }
}