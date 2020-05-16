using Aggregail;

namespace EventSourcing.Demo.Robots
{
    public sealed class RobotEdited
    {
        public static readonly EventType<RobotEdited> EventType = "RobotEdited";

        public string? Name { get; set; }
        public RobotApplication? Application { get; set; }

        public static RobotEdited Create(string? name, RobotApplication? application) =>
            new RobotEdited
            {
                Name = name,
                Application = application
            };
    }
}