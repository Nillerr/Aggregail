using Aggregail;

namespace EventSourcing.Demo.Robots
{
    public sealed class RobotRegistered
    {
        public static readonly EventType<RobotRegistered> EventType = "RobotRegistered";
        
        public RobotRegistrationId RegistrationId { get; set; }
        public EndUserId EndUserId { get; set; }
        public string? Name { get; set; }
        public RobotApplication? Application { get; set; }

        public static RobotRegistered Create(
            RobotRegistrationId registrationId,
            EndUserId endUserId,
            string? name,
            RobotApplication? application
        ) =>
            new RobotRegistered
            {
                RegistrationId = registrationId,
                EndUserId = endUserId,
                Name = name,
                Application = application
            };
    }
}