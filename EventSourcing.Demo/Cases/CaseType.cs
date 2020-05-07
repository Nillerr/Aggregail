using System;

namespace EventSourcing.Demo.Cases
{
    [JsonKnownTypes(typeof(Support), typeof(Service))]
    public abstract class CaseType
    {
        private CaseType()
        {
        }

        public abstract TResult Apply<TResult>(
            Func<Support, TResult> support,
            Func<Service, TResult> service
        );

        [JsonDiscriminator("support")]
        public sealed class Support : CaseType
        {
            public Support()
            {
                Robot = null;
            }
            
            public Support(CaseRobot robot)
            {
                Robot = robot;
            }
            
            public CaseRobot? Robot { get; }
            
            public override TResult Apply<TResult>(
                Func<Support, TResult> support,
                Func<Service, TResult> service
            ) => support(this);
        }

        [JsonDiscriminator("service")]
        public sealed class Service : CaseType
        {
            public Service()
            {
                Robot = null;
            }
            
            public Service(CaseRobot robot)
            {
                Robot = robot;
            }

            public CaseRobot? Robot { get; }

            public override TResult Apply<TResult>(
                Func<Support, TResult> support,
                Func<Service, TResult> service
            ) => service(this);
        }
    }
}