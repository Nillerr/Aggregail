using System;
using Aggregail.Newtonsoft.Json;

namespace EventSourcing.Demo.Robots.Serialization
{
    public sealed class RobotIdConverter : ValueObjectConverter<RobotId, Guid>
    {
        public RobotIdConverter()
            : base(value => new RobotId(value))
        {
        }
    }
}