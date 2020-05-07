using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public sealed class Robot : Aggregate<RobotId, Robot>,
        IApplies<RobotRegisteredByUser>,
        IApplies<RobotModifiedByUser>,
        IApplies<RobotImported>
    {
        private static readonly AggregateConfiguration<RobotId, Robot> Configuration =
            new AggregateConfiguration<RobotId, Robot>("robot")
                .Constructs(RobotImported.EventType, (id, e) => new Robot(id, e))
                .Applies(RobotRegisteredByUser.EventType)
                .Applies(RobotModifiedByUser.EventType)
                .Applies(RobotImported.EventType);

        public static Robot Imported(
            RobotId id,
            SerialNumber serialNumber,
            RobotApplication? application,
            SoftwareVersionId? softwareVersionId,
            EndUserId? registeredToId,
            DistributorId? distributedById
        )
        {
            var e = new RobotImported(serialNumber, application, softwareVersionId, registeredToId, distributedById);
            
            var robot = new Robot(id, e);
            robot.Append(id.Value, RobotImported.EventType, e);
            return robot;
        }

        public static Task<Robot?> FromAsync(IEventStoreReader reader, RobotId id)
        {
            return reader.AggregateAsync(id, Configuration);
        }

        private Robot(RobotId id, RobotImported e)
            : base(id)
        {
            SerialNumber = e.SerialNumber;
            
            InitializeRegistrations(e);
        }

        public SerialNumber SerialNumber { get; private set; }

        public ImmutableList<RobotRegistration> Registrations { get; private set; } =
            ImmutableList<RobotRegistration>.Empty;

        public Task CommitAsync(IEventStoreAppender appender)
        {
            return CommitAsync(appender, Configuration);
        }

        private void InitializeRegistrations(RobotImported e)
        {
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

        public void Apply(RobotRegisteredByUser e)
        {
            var latestRegistration = Registrations.Last();
            latestRegistration.Apply(
                registered => throw new InvalidOperationException("Robot is already registered"),
                unregistered =>
                {
                    var distribution = unregistered.Distributions.Last();
                    
                    var distributedById = distribution.Apply<DistributorId?>(
                        distributed => distributed.DistributedById,
                        inStock => null
                    );
                    
                    var registered = new RobotRegistration.Registered(e, distributedById);
                    Registrations = Registrations.Add(registered);
                } 
            );
        }

        public void Apply(RobotModifiedByUser e)
        {
            var latestRegistration = Registrations.Last();
            latestRegistration.Apply(
                registered => registered.Apply(e),
                unregistered => throw new InvalidOperationException("Robot is not registered")
            );
        }

        public void Apply(RobotImported e)
        {
            SerialNumber = e.SerialNumber;
            
            ApplyRobotImportedToRegistrations(e);
        }

        private void ApplyRobotImportedToRegistrations(RobotImported e)
        {
            var latestRegistration = Registrations.Last();
            latestRegistration.Apply(
                registered =>
                {
                    if (e.RegisteredToId == null)
                    {
                        var reg = new RobotRegistration.Unregistered(e);
                        Registrations = Registrations.Add(reg);
                    }
                    else if (e.RegisteredToId.Value == registered.RegisteredToId)
                    {
                        registered.Apply(e);
                    }
                    else
                    {
                        var reg = new RobotRegistration.Registered(e, e.RegisteredToId.Value);
                        Registrations = Registrations.Add(reg);
                    }
                },
                unregistered =>
                {
                    if (e.RegisteredToId == null)
                    {
                        unregistered.Apply(e);
                    }
                    else
                    {
                        var reg = new RobotRegistration.Registered(e, e.RegisteredToId.Value);
                        Registrations = Registrations.Add(reg);
                    }
                }
            );
        }
    }
}