using System;
using JetBrains.Annotations;

namespace EventSourcing.Demo.Cases
{
    [JsonKnownTypes(typeof(Distributed), typeof(InStock))]
    public abstract class RobotDistribution : IUnion<RobotDistribution.Distributed, RobotDistribution.InStock>
    {
        private RobotDistribution()
        {
        }

        public abstract TResult Apply<TResult>(
            [InstantHandle] Func<Distributed, TResult> distributed,
            [InstantHandle] Func<InStock, TResult> inStock
        );

        [JsonDiscriminator("distributed")]
        public sealed class Distributed : RobotDistribution
        {
            public Distributed(DistributorId distributedById)
            {
                DistributedById = distributedById;
            }

            public DistributorId DistributedById { get; private set; }

            public void Apply(RobotImported e)
            {
                // Nothing
            }

            public override TResult Apply<TResult>(
                Func<Distributed, TResult> distributed,
                Func<InStock, TResult> inStock
            ) => distributed(this);
        }

        [JsonDiscriminator("in_stock")]
        public sealed class InStock : RobotDistribution
        {
            public InStock()
            {
                // Nothing
            }

            public void Apply(RobotImported e)
            {
                // Nothing
            }
            
            public override TResult Apply<TResult>(
                Func<Distributed, TResult> distributed,
                Func<InStock, TResult> inStock
            ) => inStock(this);
        }
    }
}