using System;

namespace Aggregail
{
    /// <summary>
    /// A <see cref="IMetadataFactory"/> singleton which always returns <c>null</c>.
    /// </summary>
    public sealed class NoMetadataFactory : IMetadataFactory
    {
        /// <summary>
        /// The singleton instance
        /// </summary>
        public static readonly NoMetadataFactory Instance = new NoMetadataFactory();
        
        private NoMetadataFactory()
        {
        }
        
        /// <inheritdoc/>
        public byte[]? MetadataFor<T>(Guid id, EventType<T> type, T data, IJsonEventSerializer serializer)
        {
            return null;
        }
    }
}