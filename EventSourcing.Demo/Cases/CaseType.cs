using System;

namespace EventSourcing.Demo.Cases
{
    [JsonKnownTypes(typeof(Support), typeof(Service), typeof(Claim))]
    public abstract class CaseType
    {
        private CaseType()
        {
        }

        public abstract TResult Apply<TResult>(
            Func<Support, TResult> support,
            Func<Service, TResult> service,
            Func<Claim, TResult> claim
        );

        [JsonDiscriminator("support")]
        public sealed class Support : CaseType
        {
            public override TResult Apply<TResult>(
                Func<Support, TResult> support,
                Func<Service, TResult> service,
                Func<Claim, TResult> claim
            ) => support(this);
        }

        [JsonDiscriminator("service")]
        public sealed class Service : CaseType
        {
            public Service()
            {
                Robot = null;
            }
            
            public Service(CaseRobot? robot)
            {
                Robot = robot;
            }

            public CaseRobot? Robot { get; }

            public override TResult Apply<TResult>(
                Func<Support, TResult> support,
                Func<Service, TResult> service,
                Func<Claim, TResult> claim
            ) => service(this);
        }

        [JsonDiscriminator("claim")]
        public sealed class Claim : CaseType
        {
            public Claim()
            {
            }
            
            public Claim(CaseRobot? robot)
            {
                Robot = robot;
            }

            public CaseRobot? Robot { get; }

            public override TResult Apply<TResult>(
                Func<Support, TResult> support,
                Func<Service, TResult> service,
                Func<Claim, TResult> claim
            ) => claim(this);
        }
    }
}