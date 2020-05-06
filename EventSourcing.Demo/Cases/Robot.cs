using System;
using System.Collections.Immutable;
using System.Linq;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public sealed class Robot : Aggregate<RobotId, Robot>
    {
        public Robot(RobotId id, RobotImported e)
            : base(id)
        {
            SerialNumber = e.SerialNumber;
            
            if (e.RegisteredToId.HasValue)
            {
                var registration = new RobotRegistration.Registered(e, e.RegisteredToId.Value);
                Registrations = Registrations.Add(registration);
            }
            else
            {
                var registration = new RobotRegistration.Unregistered(e);
                Registrations = Registrations.Add(registration);
            }
        }

        public SerialNumber SerialNumber { get; private set; }

        public ImmutableList<RobotRegistration> Registrations { get; private set; } =
            ImmutableList<RobotRegistration>.Empty;

        public static Robot Imported(RobotId id, SerialNumber serialNumber, Application application, EndUserId registeredToId)
        {
            var e = new RobotImported(serialNumber, application, registeredToId);
            
            var robot = new Robot(id, e);
            robot.Append(id.Value, RobotImported.EventType, e);
            return robot;
        }

        public void Apply(RobotRegisteredByUser e)
        {
            var latestRegistration = Registrations.Last();
            latestRegistration.Apply(
                registration => throw new InvalidOperationException("Robot is already registered"),
                unregistration =>
                {
                    var registration = new RobotRegistration.Registered(e);
                    Registrations = Registrations.Add(registration);
                } 
            );
        }

        public void Apply(RobotModifiedByUser e)
        {
            var latestRegistration = Registrations.Last();
            latestRegistration.Apply(
                registration => registration.Apply(e),
                unregistration => throw new InvalidOperationException("Robot is not registered")
            );
        }

        public void Apply(RobotImported e)
        {
            SerialNumber = e.SerialNumber;
            
            var latestRegistration = Registrations.Last();
            latestRegistration.Apply(
                registration =>
                {
                    if (e.RegisteredToId == null)
                    {
                        var reg = new RobotRegistration.Unregistered(e);
                        Registrations = Registrations.Add(reg);
                    }
                    else if (e.RegisteredToId.Value == registration.RegisteredToId)
                    {
                        registration.Apply(e);
                    }
                    else
                    {
                        var reg = new RobotRegistration.Registered(e, e.RegisteredToId.Value);
                        Registrations = Registrations.Add(reg);
                    }
                },
                unregistration => unregistration.Apply(e)
            );
        }
    }
}