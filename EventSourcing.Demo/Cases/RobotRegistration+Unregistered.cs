using System;

namespace EventSourcing.Demo.Cases
{
    public partial class RobotRegistration
    {
        [JsonDiscriminator("unregistered")]
        public sealed class Unregistered : RobotRegistration
        {
            public Unregistered(RobotImported e)
                : base(e, e.DistributedById)
            {
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
            ) => unregistered(this);
        }
    }
}