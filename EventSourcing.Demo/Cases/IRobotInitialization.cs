namespace EventSourcing.Demo.Cases
{
    public interface IRobotInitialization
    {
        Application? Application { get; }
        SoftwareVersionId? SoftwareVersionId { get; }
        
        RobotRegistrar Registrar();
    }
}