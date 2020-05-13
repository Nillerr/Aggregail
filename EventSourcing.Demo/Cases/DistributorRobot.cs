using System.Linq;

namespace EventSourcing.Demo.Cases
{
    public sealed class DistributorRobot
    {
        public DistributorRobot(Robot robot, RobotRegistration registration, RobotDistribution.Distributed distribution)
        {
            Id = robot.Id;

            SerialNumber = robot.SerialNumber;

            Application = registration.Application;
            SoftwareVersionId = registration.SoftwareVersionId;
        }

        public RobotId Id { get; }

        public SerialNumber SerialNumber { get; }

        public RobotApplication? Application { get; }
        public SoftwareVersionId? SoftwareVersionId { get; }

        public static DistributorRobot? From(Robot robot, DistributorId distributedById)
        {
            var latest = (
                from registration in robot.Registrations
                from distribution in registration.Distributions
                let distributed = distribution.Apply(
                    distributed => distributed,
                    inStock => null
                )
                where distributed?.DistributedById == distributedById
                select new
                {
                    Registration = registration,
                    Distribution = distributed
                }
            ).LastOrDefault();

            if (latest == null)
            {
                return null;
            }
            
            return new DistributorRobot(robot, latest.Registration, latest.Distribution);
        }
    }
}