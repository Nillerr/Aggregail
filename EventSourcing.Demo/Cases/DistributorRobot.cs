using System;
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

        public static DistributorRobot From(Robot robot, DistributorId distributedById)
        {
            var foo = (
                from reg in robot.Registrations
                from dist in reg.Distributions
                let distributed = dist.Apply(
                    distributed => distributed,
                    inStock => null
                )
                where distributed?.DistributedById == distributedById
                select new
                {
                    Registration = reg,
                    Distribution = distributed
                }
            ).LastOrDefault();

            if (foo == null)
            {
                throw new InvalidOperationException("The robot has never been distributed by the distributor");
            }
            
            return new DistributorRobot(robot, foo.Registration, foo.Distribution);
        }
    }
}