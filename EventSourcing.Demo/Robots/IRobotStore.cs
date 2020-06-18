using Aggregail;

namespace EventSourcing.Demo.Robots
{
    public interface IRobotStore : IAggregateStore<RobotId, Robot>
    {
    }
}