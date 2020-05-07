using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public sealed class RobotModifiedByUser
    {
        public static readonly EventType<RobotModifiedByUser> EventType = "RobotModifiedByUser";

        public RobotModifiedByUser(
            UserId modifiedById,
            string name,
            string integrator,
            string description
        )
        {
            ModifiedById = modifiedById;
            
            Name = name;

            Integrator = integrator;
            Description = description;
        }

        public UserId ModifiedById { get; }
        
        public string Name { get; }

        public string Integrator { get; }
        
        public string Description { get; }
        
        public RobotApplication? Application { get; }
        public SoftwareVersionId? SoftwareVersionId { get; }
    }
}