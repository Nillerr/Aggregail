using System;
using Aggregail;

namespace EventSourcing.Demo
{
    public sealed class UserMetadataFactory : IMetadataFactory
    {
        public UserMetadataFactory(string username)
        {
            Username = username;
        }

        public string Username { get; }

        public byte[]? MetadataFor<T>(Guid id, EventType<T> type, T data, IJsonEventSerializer serializer)
        {
            return serializer.Serialize(new {Username});
        }
    }
}