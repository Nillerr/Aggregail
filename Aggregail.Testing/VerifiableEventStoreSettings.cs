namespace Aggregail.Testing
{
    /// <summary>
    /// Configuration options for the <see cref="VerifiableEventStore"/> class.
    /// </summary>
    public sealed class VerifiableEventStoreSettings
    {
        /// <summary>
        /// Creates an instance of the <see cref="VerifiableEventStoreSettings"/> class.
        /// </summary>
        /// <param name="eventSerializer">Serializer to use when serializing events.</param>
        public VerifiableEventStoreSettings(IJsonEventSerializer eventSerializer)
        {
            EventSerializer = eventSerializer;
        }
        
        /// <summary>
        /// The serializer to use when serializing events.
        /// </summary>
        public IJsonEventSerializer EventSerializer { get; set; }
        
        /// <summary>
        /// Resolves names of streams for aggregates.
        /// </summary>
        public IStreamNameResolver StreamNameResolver { get; set; } = TokenizedStreamNameResolver.Default;
        
        /// <summary>
        /// The service responsible for creating metadata for a given event.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="NoMetadataFactory"/>.
        /// </remarks>
        public IMetadataFactory MetadataFactory { get; set; } = NoMetadataFactory.Instance;
    }
}