using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public sealed class RobotImported
    {
        public static readonly EventType<RobotImported> EventType = "RobotImported";
        
        public RobotImported(SerialNumber serialNumber, Application application, EndUserId? registeredToId)
        {
            SerialNumber = serialNumber;
            Application = application;
            RegisteredToId = registeredToId;
        }

        public SerialNumber SerialNumber { get; }
        
        public Application? Application { get; }
        
        public SoftwareVersionId? SoftwareVersionId { get; }
        
        public EndUserId? RegisteredToId { get; }
    }
}