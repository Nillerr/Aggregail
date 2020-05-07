using System;
using System.Linq;

namespace EventSourcing.Demo.Cases
{
    public sealed class EndUserRobot
    {
        public EndUserRobot(Robot robot, RobotRegistration.Registered registration)
        {
            Id = robot.Id;

            SerialNumber = robot.SerialNumber;

            Name = registration.Name;
            Integrator = registration.Integrator;
            Description = registration.Description;

            Application = registration.Application;
            SoftwareVersionId = registration.SoftwareVersionId;
        }

        public RobotId Id { get; }

        public SerialNumber SerialNumber { get; }

        public string Name { get; }
        public string Integrator { get; }
        public string Description { get; }

        public RobotApplication? Application { get; }
        public SoftwareVersionId? SoftwareVersionId { get; }

        public static EndUserRobot From(Robot robot, EndUserId registeredToId)
        {
            var registration = (
                from reg in robot.Registrations
                
                let registered = reg.Apply(
                        registered => registered,
                        unregistered => null
                    )
                    
                where registered?.RegisteredToId == registeredToId
                select registered
            ).LastOrDefault();

            if (registration == null)
            {
                throw new InvalidOperationException("The robot has never been registered to the end user");
            }

            return new EndUserRobot(robot, registration);
        }
    }
}