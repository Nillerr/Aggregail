using System;
using EventSourcing.Demo.Cases.Events;
using JetBrains.Annotations;

namespace EventSourcing.Demo.Cases
{
    [JsonKnownTypes(typeof(EndUser), typeof(Distributor), typeof(Service))]
    public abstract class CaseCommentAuthor
    {
        public abstract TResult Apply<TResult>(
            [InstantHandle] Func<EndUser, TResult> endUser,
            [InstantHandle] Func<Distributor, TResult> distributor,
            [InstantHandle] Func<Service, TResult> service
        );

        [JsonDiscriminator("end_user")]
        public sealed class EndUser : CaseCommentAuthor
        {
            public EndUser(EndUserId id, UserId userId)
            {
                Id = id;
                UserId = userId;
            }

            public EndUserId Id { get; }
            public UserId UserId { get; }

            public override TResult Apply<TResult>(
                Func<EndUser, TResult> endUser,
                Func<Distributor, TResult> distributor,
                Func<Service, TResult> service
            ) => endUser(this);
        }

        [JsonDiscriminator("distributor")]
        public sealed class Distributor : CaseCommentAuthor
        {
            public Distributor(DistributorId id, UserId userId, RecipientType recipient)
            {
                Id = id;
                UserId = userId;
                Recipient = recipient;
            }

            public DistributorId Id { get; }
            public UserId UserId { get; }
            public RecipientType Recipient { get; }

            public enum RecipientType
            {
                Everyone,
                Service
            }

            public override TResult Apply<TResult>(
                Func<EndUser, TResult> endUser,
                Func<Distributor, TResult> distributor,
                Func<Service, TResult> service
            ) => distributor(this);
        }

        [JsonDiscriminator("service")]
        public sealed class Service : CaseCommentAuthor
        {
            public Service(SystemUserId systemUserId, RecipientType recipient)
            {
                SystemUserId = systemUserId;
                Recipient = recipient;
            }

            public static Service Create(PortalCommentImported e)
            {
                var recipientType = ToRecipientType(e.Audience);
                return new Service(e.OwnerId, recipientType);
            }

            private static RecipientType ToRecipientType(PortalCommentImported.PortalCommentAudience audience) =>
                audience switch
                {
                    PortalCommentImported.PortalCommentAudience.All => RecipientType.Everyone,
                    PortalCommentImported.PortalCommentAudience.EndUser => RecipientType.EndUser,
                    PortalCommentImported.PortalCommentAudience.Distributor => RecipientType.Distributor,
                    PortalCommentImported.PortalCommentAudience.DistributorAndSI => RecipientType.Distributor,
                    PortalCommentImported.PortalCommentAudience.SubDistributorOrSI => RecipientType.Distributor,
                    _ => throw new ArgumentOutOfRangeException(nameof(audience), audience, null)
                };

            public SystemUserId SystemUserId { get; }
            public RecipientType Recipient { get; }

            public enum RecipientType
            {
                Everyone,
                Distributor,
                EndUser
            }

            public override TResult Apply<TResult>(
                Func<EndUser, TResult> endUser,
                Func<Distributor, TResult> distributor,
                Func<Service, TResult> service
            ) => service(this);
        }
    }
}