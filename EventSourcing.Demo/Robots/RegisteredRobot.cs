using EventSourcing.Demo.Robots.CRM;

namespace EventSourcing.Demo.Robots
{
    public sealed class RegisteredRobot
    {
        public RegisteredRobot(Robot robot, RobotRegistration.Registration registration)
        {
            Id = robot.Id;
            Product = robot.Product;
            SerialNumber = robot.SerialNumber;
            Name = registration.Name;
            Application = registration.Application;
        }

        public RobotId Id { get; }
        public RobotProduct Product { get; }
        public SerialNumber SerialNumber { get; }
        public string Name { get; }
        public RobotApplication? Application { get; }
    }
}