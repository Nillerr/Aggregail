namespace EventSourcing.Demo.Cases
{
    public sealed class CaseRobot
    {
        public CaseRobot(RobotId id)
        {
            Id = id;
        }

        public RobotId Id { get; }
    }
}