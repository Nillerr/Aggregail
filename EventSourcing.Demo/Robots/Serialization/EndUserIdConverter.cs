using System;
using Aggregail.Newtonsoft.Json;

namespace EventSourcing.Demo.Robots.Serialization
{
    public sealed class EndUserIdConverter : ValueObjectConverter<EndUserId, Guid>
    {
        public EndUserIdConverter()
            : base(value => new EndUserId(value))
        {
        }
    }
}