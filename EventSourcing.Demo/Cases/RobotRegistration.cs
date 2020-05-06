using System;
using JetBrains.Annotations;

namespace EventSourcing.Demo.Cases
{
    [JsonKnownTypes(typeof(Registered), typeof(Unregistered))]
    public abstract class RobotRegistration : IUnion<RobotRegistration.Registered, RobotRegistration.Unregistered>
    {
        private RobotRegistration()
        {
        }

        public abstract TResult Apply<TResult>(
            [InstantHandle] Func<Registered, TResult> registration,
            [InstantHandle] Func<Unregistered, TResult> unregistration
        );

        [JsonDiscriminator("registered")]
        public sealed class Registered : RobotRegistration
        {
            public Registered(RobotImported e, EndUserId registeredToId)
            {
                RegisteredToId = registeredToId;

                Registrar = RobotRegistrar.Import.Instance;
                
                Name = string.Empty;
                Integrator = string.Empty;
                Description = string.Empty;

                Application = e.Application;
                SoftwareVersionId = e.SoftwareVersionId;
            }

            public Registered(RobotRegisteredByUser e)
            {
                RegisteredToId = e.RegisteredToId;
                
                Registrar = new RobotRegistrar.User(e.RegisteredById);

                Name = e.Name;
                Integrator = e.Integrator;
                Description = e.Description;
            }

            public EndUserId RegisteredToId { get; }

            public RobotRegistrar Registrar { get; }
            
            public UserId? ModifiedById { get; private set; }

            public string Name { get; private set; }
            public string Integrator { get; private set; }
            public string Description { get; private set; }
            
            public Application? Application { get; private set; }
            public SoftwareVersionId? SoftwareVersionId { get; private set; }

            public void Apply(RobotModifiedByUser e)
            {
                ModifiedById = e.ModifiedById;
                
                Name = e.Name;
                Integrator = e.Integrator;
                Description = e.Description;

                Application = e.Application;
                SoftwareVersionId = e.SoftwareVersionId;
            }

            public void Apply(RobotImported e)
            {
                Application = e.Application;
                SoftwareVersionId = e.SoftwareVersionId;
            }

            public override TResult Apply<TResult>(
                Func<Registered, TResult> registration,
                Func<Unregistered, TResult> unregistration
            ) => registration(this);
        }

        [JsonDiscriminator("unregistered")]
        public sealed class Unregistered : RobotRegistration
        {
            public Unregistered(RobotImported e)
            {
                Registrar = RobotRegistrar.Import.Instance;

                Application = e.Application;
                SoftwareVersionId = e.SoftwareVersionId;
            }

            public RobotRegistrar Registrar { get; }
            
            public Application? Application { get; private set; }
            public SoftwareVersionId? SoftwareVersionId { get; private set; }

            public void Apply(RobotImported e)
            {
                Application = e.Application;
                SoftwareVersionId = e.SoftwareVersionId;
            }

            public override TResult Apply<TResult>(
                Func<Registered, TResult> registration,
                Func<Unregistered, TResult> unregistration
            ) => unregistration(this);
        }
    }
}