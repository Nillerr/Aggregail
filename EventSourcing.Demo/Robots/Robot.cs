using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Aggregail;
using EventSourcing.Demo.Robots.CRM;

namespace EventSourcing.Demo.Robots
{
    public sealed class Robot : Aggregate<RobotId, Robot>
    {
        private static readonly AggregateConfiguration<RobotId, Robot> Configuration =
            new AggregateConfiguration<RobotId, Robot>("Robot")
                .Constructs(RobotImported.EventType, (id, e) => new Robot(id, e))
                .Applies(RobotImported.EventType, (robot, e) => robot.Apply(e))
                .Applies(RobotRegistered.EventType, (robot, e) => robot.Apply(e))
                .Applies(RobotEdited.EventType, (robot, e) => robot.Apply(e))
                .Applies(RobotUnregistered.EventType, (robot, e) => robot.Apply(e));

        private Robot(RobotId id, RobotImported e)
            : base(id)
        {
            SerialNumber = new SerialNumber(e.Entity.C2RurName);
            Product = e.Entity.RobotProduct();

            Registrations = ImmutableList<RobotRegistration>.Empty;
            ImportRegistrations(e);
        }

        public RobotProduct Product { get; private set; }
        
        public SerialNumber SerialNumber { get; private set; }

        public ImmutableList<RobotRegistration> Registrations { get; private set; }

        public static Robot Import(RobotImported.RobotEntity entity)
        {
            var id = new RobotId(entity.C2RurRobotsid);
            var e = RobotImported.Create(entity);
            var robot = new Robot(id, e);
            robot.Append(id.Value, RobotImported.EventType, e);
            return robot;
        }

        public static Task<Robot?> FromAsync(IEventStore store, RobotId id) => store.AggregateAsync(id, Configuration);

        public Task CommitAsync(IEventStore store) => CommitAsync(store, Configuration);

        public RobotRegistration.Registration? LatestRegistration => (
            from registration in Registrations
            let reg = registration.Apply(r => r, u => null)
            select reg!
        ).LastOrDefault();

        public RobotRegistration.Registration? LatestRegistrationFor(EndUserId endUserId)
        {
            return (
                from registration in Registrations
                let reg = registration.Apply(r => r, u => null)
                where reg?.EndUserId == endUserId
                select reg!
            ).LastOrDefault();
        }

        public void Register(EndUserId endUserId, string? name, RobotApplication? application)
        {
            void AddRegistration()
            {
                var registrationId = new RobotRegistrationId(Guid.NewGuid());
                var e = RobotRegistered.Create(registrationId, endUserId, name, application);

                Apply(e);
                Append(registrationId.Value, RobotRegistered.EventType, e);
            }

            var latestRegistration = Registrations.LastOrDefault();
            if (latestRegistration == null)
            {
                AddRegistration();
            }
            else
            {
                latestRegistration.Apply(
                    registration => throw new ValidationException("Robot is already registered"),
                    unregistration => AddRegistration()
                );
            }
        }

        public void Unregister(EndUserId endUserId)
        {
            var latestRegistration = Registrations.LastOrDefault();
            if (latestRegistration == null)
            {
                throw new ValidationException("Robot is not registered");
            }

            latestRegistration.Apply(
                registration =>
                {
                    if (registration.EndUserId == endUserId)
                    {
                        var e = RobotUnregistered.Create();
                        Apply(e);
                        Append(Guid.NewGuid(), RobotUnregistered.EventType, e);
                    }
                    else
                    {
                        throw new ValidationException("Robot is registered to somebody else");
                    }
                },
                unregistration => throw new ValidationException("Robot is not registered")
            );
        }

        public void Edit(EndUserId endUserId, string? name, RobotApplication? application)
        {
            var latestRegistration = Registrations.LastOrDefault();
            if (latestRegistration == null)
            {
                throw new ValidationException("Robot is not registered");
            }

            latestRegistration.Apply(
                registration =>
                {
                    if (registration.EndUserId != endUserId)
                    {
                        throw new ValidationException("Robot is registered to somebody else");
                    }

                    var e = RobotEdited.Create(name, application);

                    Apply(e);
                    Append(Guid.NewGuid(), RobotEdited.EventType, e);
                },
                unregistration => throw new ValidationException("Robot is not registered")
            );
        }

        private void Apply(RobotImported e)
        {
            SerialNumber = new SerialNumber(e.Entity.C2RurName);
            ImportRegistrations(e);
        }

        private void ImportRegistrations(RobotImported e)
        {
            void AddRegistration(Guid endUserId)
            {
                var newRegistration = new RobotRegistration.Registration(this, e, endUserId);
                Registrations = Registrations.Add(newRegistration);
            }

            var latestRegistration = Registrations.LastOrDefault();
            if (latestRegistration == null)
            {
                if (e.Entity.C2RurEnduserValue != null)
                {
                    AddRegistration(e.Entity.C2RurEnduserValue.Value);
                }
            }
            else
            {
                latestRegistration.Apply(
                    registration =>
                    {
                        if (registration.EndUserId.Value == e.Entity.C2RurEnduserValue)
                        {
                            registration.Apply(e);
                        }
                        else if (e.Entity.C2RurEnduserValue.HasValue)
                        {
                            AddRegistration(e.Entity.C2RurEnduserValue.Value);
                        }
                        else
                        {
                            Registrations = Registrations.Add(RobotRegistration.Unregistration.Instance);
                        }
                    },
                    unregistration =>
                    {
                        if (e.Entity.C2RurEnduserValue.HasValue)
                        {
                            AddRegistration(e.Entity.C2RurEnduserValue.Value);
                        }
                    }
                );
            }
        }

        private void Apply(RobotRegistered e)
        {
            void AddRegistration()
            {
                var registration = new RobotRegistration.Registration(this, e);
                Registrations = Registrations.Add(registration);
            }

            var latestRegistration = Registrations.LastOrDefault();
            if (latestRegistration == null)
            {
                AddRegistration();
            }
            else
            {
                latestRegistration.Apply(
                    registration => throw new InvalidOperationException("Robot is already registered"),
                    unregistration => AddRegistration()
                );
            }
        }

        private void Apply(RobotUnregistered e)
        {
            var latestRegistration = Registrations.LastOrDefault();
            if (latestRegistration == null)
            {
                throw new InvalidOperationException("Robot is not already registered");
            }

            latestRegistration.Apply(
                registration => { Registrations = Registrations.Add(RobotRegistration.Unregistration.Instance); },
                unregistration => throw new InvalidOperationException("Robot is not already registered")
            );
        }

        private void Apply(RobotEdited e)
        {
            var latestRegistration = Registrations.LastOrDefault();
            if (latestRegistration == null)
            {
                throw new InvalidOperationException("Robot is not registered");
            }

            latestRegistration.Apply(
                registration => registration.Apply(this, e),
                unregistration => throw new InvalidOperationException("Robot is not registered")
            );
        }
    }
}