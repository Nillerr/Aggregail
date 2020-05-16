using System;
using Aggregail.Newtonsoft.Json;

namespace EventSourcing.Demo.Robots.Serialization
{
    public sealed class RobotRegistrationIdConverter : ValueObjectConverter<RobotRegistrationId, Guid>
    {
        public RobotRegistrationIdConverter()
            : base(value => new RobotRegistrationId(value))
        {
        }
    }
}