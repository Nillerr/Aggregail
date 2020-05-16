namespace EventSourcing.Demo.Robots
{
    public sealed class RobotRegistrationChanged
    {
        public RobotRegistrationId RegistrationId { get; private set; }
        public string Name { get; private set; }
    }
}