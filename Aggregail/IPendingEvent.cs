using System;

namespace Aggregail
{
    /// <summary>
    /// An event which hasn't been committed yet
    /// </summary>
    public interface IPendingEvent
    {
        /// <summary>
        /// Id of the event
        /// </summary>
        Guid Id { get; }
        
        /// <summary>
        /// Type of the event
        /// </summary>
        string Type { get; }
        
        /// <summary>
        /// Data of the event, serialized as JSON.
        /// </summary>
        /// <param name="serializer">The JSON serializer to serialize the event data with.</param>
        /// <returns>JSON of the event data, encoded as an UTF-8 string.</returns>
        byte[] Data(IJsonEventSerializer serializer);

        /// <summary>
        /// Metadata for the event, using the <see cref="IMetadataFactory"/> specified by <paramref name="factory"/>.
        /// </summary>
        /// <param name="factory">The metadata factory</param>
        /// <param name="serializer">The JSON serializer to serialize the metadata with.</param>
        /// <returns>JSON of the metadata, encoded as an UTF-8 string.</returns>
        byte[]? Metadata(IMetadataFactory factory, IJsonEventSerializer serializer);
    }
}