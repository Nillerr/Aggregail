using System;

namespace EventSourcing.Demo.Cases
{
    public enum CaseTypeKind
    {
        Support,
        Service,
        Claim
    }
    
    [JsonKnownTypes(nameof(Kind), typeof(Support), typeof(Service), typeof(Claim))]
    public abstract class CaseType
    {
        private CaseType()
        {
        }
        
        public abstract CaseTypeKind Kind { get; }

        public abstract TResult Apply<TResult>(
            Func<Support, TResult> support,
            Func<Service, TResult> service,
            Func<Claim, TResult> claim
        );

        [JsonDiscriminator(CaseTypeKind.Support)]
        public sealed class Support : CaseType
        {
            public override CaseTypeKind Kind { get; } = CaseTypeKind.Support;

            public override TResult Apply<TResult>(
                Func<Support, TResult> support,
                Func<Service, TResult> service,
                Func<Claim, TResult> claim
            ) => support(this);
        }

        [JsonDiscriminator(CaseTypeKind.Service)]
        public sealed class Service : CaseType
        {
            public Service(RobotRegistrationId robotRegistrationId)
            {
                RobotRegistrationId = robotRegistrationId;
            }

            public RobotRegistrationId RobotRegistrationId { get; }

            public override CaseTypeKind Kind { get; } = CaseTypeKind.Service;

            public override TResult Apply<TResult>(
                Func<Support, TResult> support,
                Func<Service, TResult> service,
                Func<Claim, TResult> claim
            ) => service(this);
        }

        [JsonDiscriminator(CaseTypeKind.Claim)]
        public sealed class Claim : CaseType
        {
            public Claim(RobotRegistrationId robotRegistrationId)
            {
                RobotRegistrationId = robotRegistrationId;
            }

            public RobotRegistrationId RobotRegistrationId { get; }

            public override CaseTypeKind Kind { get; } = CaseTypeKind.Claim;

            public override TResult Apply<TResult>(
                Func<Support, TResult> support,
                Func<Service, TResult> service,
                Func<Claim, TResult> claim
            ) => claim(this);
        }
    }
}