using Aggregail;
using EventSourcing.Demo.Robots.CRM;

namespace EventSourcing.Demo.Robots
{
    public sealed class RobotImported
    {
        public static readonly EventType<RobotImported> EventType = "RobotImported";
        
        public RobotEntity Entity { get; set; }

        public static RobotImported Create(RobotEntity entity) =>
            new RobotImported
            {
                Entity = entity
            };
    }
}