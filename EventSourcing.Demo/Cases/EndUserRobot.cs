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

        public static EndUserRobot? From(Robot robot, EndUserId registeredToId)
        {
            var latest = (
                from registration in robot.Registrations
                
                let registered = registration.Apply(
                        registered => registered,
                        unregistered => null
                    )
                    
                where registered?.RegisteredToId == registeredToId
                select new { Registration = registered }
            ).LastOrDefault();

            if (latest == null)
            {
                return null;
            }

            return new EndUserRobot(robot, latest.Registration);
        }
    }
}