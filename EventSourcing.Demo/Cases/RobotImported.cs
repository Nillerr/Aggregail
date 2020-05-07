using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public sealed class RobotImported : IRobotInitialization
    {
        public static readonly EventType<RobotImported> EventType = "RobotImported";

        public RobotImported(
            SerialNumber serialNumber,
            RobotApplication? application,
            SoftwareVersionId? softwareVersionId,
            EndUserId? registeredToId,
            DistributorId? distributedById
        )
        {
            SerialNumber = serialNumber;
            Application = application;
            SoftwareVersionId = softwareVersionId;
            RegisteredToId = registeredToId;
            DistributedById = distributedById;
        }

        public SerialNumber SerialNumber { get; }

        public RobotApplication? Application { get; }

        public SoftwareVersionId? SoftwareVersionId { get; }

        public EndUserId? RegisteredToId { get; }

        public DistributorId? DistributedById { get; }

        public RobotRegistrar Registrar() => RobotRegistrar.Import.Instance;
    }
}