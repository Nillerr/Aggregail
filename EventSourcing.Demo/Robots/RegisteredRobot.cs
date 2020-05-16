namespace EventSourcing.Demo.Robots
{
    public sealed class RegisteredRobot
    {
        public RegisteredRobot(Robot robot, RobotRegistration.Registration registration)
        {
            Id = robot.Id;
            SerialNumber = robot.SerialNumber;
            Name = registration.Name;
        }

        public RobotId Id { get; }
        public SerialNumber SerialNumber { get; }
        public string Name { get; }
    }
}