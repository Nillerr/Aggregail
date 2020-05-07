using System;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;

namespace EventSourcing.Demo.Cases
{
    [JsonKnownTypes(typeof(Registered), typeof(Unregistered))]
    public abstract partial class RobotRegistration : 
        IUnion<RobotRegistration.Registered, RobotRegistration.Unregistered>
    {
        private RobotRegistration(IRobotInitialization e, DistributorId? distributedById)
        {
            Registrar = e.Registrar();
            
            Application = e.Application;
            SoftwareVersionId = e.SoftwareVersionId;
            
            InitializeDistributions(distributedById);
        }

        public RobotRegistrar Registrar { get; }

        public Application? Application { get; private set; }
        public SoftwareVersionId? SoftwareVersionId { get; private set; }

        public ImmutableList<RobotDistribution> Distributions { get; private set; } =
            ImmutableList<RobotDistribution>.Empty;

        private void InitializeDistributions(DistributorId? distributedById)
        {
            if (distributedById.HasValue)
            {
                var dist = new RobotDistribution.Distributed(distributedById.Value);
                Distributions = Distributions.Add(dist);
            }
            else
            {
                var dist = new RobotDistribution.InStock();
                Distributions = Distributions.Add(dist);
            }
        }

        public virtual void Apply(RobotImported e)
        {
            ApplyRobotImportedToDistributions(e);
        }

        private void ApplyRobotImportedToDistributions(RobotImported e)
        {
            var latestDistribution = Distributions.Last();
            latestDistribution.Apply(
                distributed =>
                {
                    if (e.DistributedById == null)
                    {
                        var dist = new RobotDistribution.InStock();
                        Distributions = Distributions.Add(dist);
                    }
                    else if (e.DistributedById.Value == distributed.DistributedById)
                    {
                        distributed.Apply(e);
                    }
                    else
                    {
                        var dist = new RobotDistribution.Distributed(e.DistributedById.Value);
                        Distributions = Distributions.Add(dist);
                    }
                },
                inStock =>
                {
                    if (e.DistributedById == null)
                    {
                        inStock.Apply(e);
                    }
                    else
                    {
                        var dist = new RobotDistribution.Distributed(e.DistributedById.Value);
                        Distributions = Distributions.Add(dist);
                    }
                }
            );
        }

        public abstract TResult Apply<TResult>(
            [InstantHandle] Func<Registered, TResult> registration,
            [InstantHandle] Func<Unregistered, TResult> unregistered
        );
    }
}