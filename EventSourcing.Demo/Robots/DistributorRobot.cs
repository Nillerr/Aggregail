using EventSourcing.Demo.Robots.CRM;

namespace EventSourcing.Demo.Robots
{
    public sealed class DistributorRobot
    {
        public DistributorRobot(Robot robot)
        {
            Id = robot.Id;
            Product = robot.Product;
            SerialNumber = robot.SerialNumber;
        }

        public RobotId Id { get; }
        public RobotProduct Product { get; }
        public SerialNumber SerialNumber { get; }
    }
}