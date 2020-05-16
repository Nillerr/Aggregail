using System;
using JetBrains.Annotations;

namespace EventSourcing.Demo.Robots
{
    public abstract class RobotRegistration
    {
        private RobotRegistration()
        {
        }
        
        public abstract void Apply(
            [InstantHandle] Action<Registration> case1,
            [InstantHandle] Action<Unregistration> case2
        );

        public abstract TResult Apply<TResult>(
            [InstantHandle] Func<Registration, TResult> case1,
            [InstantHandle] Func<Unregistration, TResult> case2
        );

        public sealed class Registration : RobotRegistration
        {
            public Registration(Robot robot, RobotImported e, Guid endUserId)
            {
                Id = new RobotRegistrationId(Guid.NewGuid());
                EndUserId = new EndUserId(endUserId);
                Name = $"{robot.Product:G} - {robot.SerialNumber}";
                Application = e.Entity.GetRobotApplication();
            }

            public Registration(Robot robot, RobotRegistered e)
            {
                Id = new RobotRegistrationId(Guid.NewGuid());
                EndUserId = e.EndUserId;
                Name = e.Name ?? $"{robot.Product:G} - {robot.SerialNumber}";
                Application = e.Application;
            }

            public RobotRegistrationId Id { get; private set; }
            public EndUserId EndUserId { get; private set; }
            public string Name { get; private set; }
            public RobotApplication? Application { get; private set; }

            public override void Apply(Action<Registration> case1, Action<Unregistration> case2) => case1(this);

            public override TResult Apply<TResult>(
                Func<Registration, TResult> case1,
                Func<Unregistration, TResult> case2
            ) => case1(this);

            public void Apply(Robot robot, RobotEdited e)
            {
                Name = e.Name ?? $"{robot.Product:G} - {robot.SerialNumber}";
                Application = e.Application;
            }

            public void Apply(RobotImported e)
            {
                Application = e.Entity.AkaApplicationTest?.GetRobotApplication();
            }
        }

        public sealed class Unregistration : RobotRegistration
        {
            public static readonly Unregistration Instance = new Unregistration();

            private Unregistration()
            {
            }
            
            public override void Apply(Action<Registration> case1, Action<Unregistration> case2) => case2(this);

            public override TResult Apply<TResult>(
                Func<Registration, TResult> case1,
                Func<Unregistration, TResult> case2
            ) => case2(this);
        }
    }
}