namespace Aggregail
{
    /// <summary>
    /// Configuration options for the <see cref="InMemoryEventStore"/> class.
    /// </summary>
    public sealed class InMemoryEventStoreSettings
    {
        /// <summary>
        /// Creates an instance of the <see cref="InMemoryEventStoreSettings"/> class.
        /// </summary>
        /// <param name="eventSerializer">Serializer to use when serializing events.</param>
        public InMemoryEventStoreSettings(IJsonEventSerializer eventSerializer)
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
    }
}