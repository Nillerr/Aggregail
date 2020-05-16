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
            public Registration(RobotRegistrationId id, EndUserId endUserId, string name)
            {
                Id = id;
                EndUserId = endUserId;
                Name = name;
            }

            public RobotRegistrationId Id { get; private set; }
            public EndUserId EndUserId { get; private set; }
            public string Name { get; private set; }

            public override void Apply(Action<Registration> case1, Action<Unregistration> case2) => case1(this);

            public override TResult Apply<TResult>(
                Func<Registration, TResult> case1,
                Func<Unregistration, TResult> case2
            ) => case1(this);

            public void Apply(RobotEdited e)
            {
                Name = e.Name;
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