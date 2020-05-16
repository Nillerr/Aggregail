using Aggregail;

namespace EventSourcing.Demo.Robots
{
    public sealed class RobotUnregistered
    {
        public static readonly EventType<RobotUnregistered> EventType = "RobotUnregistered";

        public static RobotUnregistered Create() => new RobotUnregistered();
    }
}