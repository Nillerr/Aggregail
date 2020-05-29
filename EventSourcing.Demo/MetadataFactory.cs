using System;
using Aggregail;
using Newtonsoft.Json;

namespace EventSourcing.Demo
{
    public sealed class MetadataFactory : IMetadataFactory
    {
        public MetadataFactory(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; }
        
        public byte[]? MetadataFor<T>(Guid id, EventType<T> type, T data, IJsonEventSerializer serializer)
        {
            var metadata = new Metadata(UserId);
            return serializer.Serialize(metadata);
        }

        private sealed class Metadata
        {
            public Metadata(string userId)
            {
                UserId = userId;
            }

            [JsonProperty("user_id")]
            public string UserId { get; }
        }
    }
}