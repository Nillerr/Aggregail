using System;

namespace EventSourcing.Demo.Robots.CRM
{
    public sealed class RobotEntity
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; }
    }
}