using System;

namespace Aggregail
{
    /// <summary>
    /// Creates metadata for a given event.
    /// </summary>
    public interface IMetadataFactory
    {
        /// <summary>
        /// Returns the metadata associated with an event matching the parameters, or <c>null</c> if no metadata is
        /// desired.
        /// </summary>
        /// <param name="id">Id of the event</param>
        /// <param name="type">Type of the event</param>
        /// <param name="data">Data of the event</param>
        /// <param name="serializer">Serializer configured by the calling <see cref="IEventStore"/>.</param>
        /// <typeparam name="T">Type of the event data</typeparam>
        /// <returns>Metadata associated with the event, or <c>null</c>.</returns>
        byte[]? MetadataFor<T>(Guid id, EventType<T> type, T data, IJsonEventSerializer serializer);
    }
}