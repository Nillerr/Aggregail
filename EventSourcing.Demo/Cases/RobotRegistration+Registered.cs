using System;

namespace EventSourcing.Demo.Cases
{
    public partial class RobotRegistration
    {
        [JsonDiscriminator("registered")]
        public sealed class Registered : RobotRegistration
        {
            public Registered(RobotImported e, EndUserId registeredToId)
                : base(e, e.DistributedById)
            {
                RegisteredToId = registeredToId;

                Name = string.Empty;
                Integrator = string.Empty;
                Description = string.Empty;
            }

            public Registered(RobotRegisteredByUser e, DistributorId? distributedById)
                : base(e, distributedById)
            {
                RegisteredToId = e.RegisteredToId;

                Name = e.Name;
                Integrator = e.Integrator;
                Description = e.Description;
            }

            public EndUserId RegisteredToId { get; }

            public UserId? ModifiedById { get; private set; }

            public string Name { get; private set; }
            public string Integrator { get; private set; }
            public string Description { get; private set; }

            public void Apply(RobotModifiedByUser e)
            {
                ModifiedById = e.ModifiedById;

                Name = e.Name;
                Integrator = e.Integrator;
                Description = e.Description;

                Application = e.Application;
                SoftwareVersionId = e.SoftwareVersionId;
            }

            public override void Apply(RobotImported e)
            {
                base.Apply(e);

                Application = e.Application;
                SoftwareVersionId = e.SoftwareVersionId;
            }

            public override TResult Apply<TResult>(
                Func<Registered, TResult> registration,
                Func<Unregistered, TResult> unregistered
            ) => registration(this);
        }
    }
}