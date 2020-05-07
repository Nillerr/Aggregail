namespace EventSourcing.Demo.Cases
{
    public interface IRobotInitialization
    {
        RobotApplication? Application { get; }
        SoftwareVersionId? SoftwareVersionId { get; }
        
        RobotRegistrar Registrar();
    }
}