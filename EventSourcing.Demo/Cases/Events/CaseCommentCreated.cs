using System;
using System.Collections.Immutable;
using EventSourcing.Demo.Framework;
using JetBrains.Annotations;

namespace EventSourcing.Demo.Cases.Events
{
    public sealed class CaseCommentCreated
    {
        public static readonly EventType<CaseCommentCreated> EventType = "CaseCommentCreated";
        
        public CaseCommentCreated(
            CaseCommentId id,
            string description,
            CaseCommentAuthor author,
            ImmutableList<CaseCommentAttachmentId> attachmentIds
        )
        {
            Id = id;
            Description = description;
            Author = author;
            AttachmentIds = attachmentIds;
        }

        public static CaseCommentCreated Create(
            CaseCommentId id,
            string description,
            Cases.CaseCommentAuthor author,
            params CaseCommentAttachmentId[] attachmentIds
        )
        {
            return new CaseCommentCreated(
                id,
                description,
                CaseCommentAuthor.Create(author),
                attachmentIds.ToImmutableList()
            );
        }

        public CaseCommentId Id { get; }
        public string Description { get; }
        public CaseCommentAuthor Author { get; }
        public ImmutableList<CaseCommentAttachmentId> AttachmentIds { get; }

        [JsonKnownTypes(typeof(EndUser), typeof(Distributor), typeof(Service))]
        public abstract class CaseCommentAuthor
        {
            public static CaseCommentAuthor Create(Cases.CaseCommentAuthor source) =>
                source.Apply<CaseCommentAuthor>(EndUser.Create, Distributor.Create, Service.Create);

            public abstract Cases.CaseCommentAuthor ToCaseCommentAuthor();

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

                public static EndUser Create(Cases.CaseCommentAuthor.EndUser source) =>
                    new EndUser(source.Id, source.UserId);

                public override Cases.CaseCommentAuthor ToCaseCommentAuthor() =>
                    new Cases.CaseCommentAuthor.EndUser(Id, UserId);

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

                public static Distributor Create(Cases.CaseCommentAuthor.Distributor source) =>
                    new Distributor(source.Id, source.UserId, ToRecipientType(source.Recipient));

                private static RecipientType ToRecipientType(Cases.CaseCommentAuthor.Distributor.RecipientType source) =>
                    source switch
                    {
                        Cases.CaseCommentAuthor.Distributor.RecipientType.Everyone => RecipientType.Everyone,
                        Cases.CaseCommentAuthor.Distributor.RecipientType.Service => RecipientType.Service,
                        _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
                    };

                private static Cases.CaseCommentAuthor.Distributor.RecipientType ToRecipientType(RecipientType source) =>
                    source switch
                    {
                        RecipientType.Everyone => Cases.CaseCommentAuthor.Distributor.RecipientType.Everyone,
                        RecipientType.Service => Cases.CaseCommentAuthor.Distributor.RecipientType.Service,
                        _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
                    };

                public override Cases.CaseCommentAuthor ToCaseCommentAuthor() =>
                    new Cases.CaseCommentAuthor.Distributor(Id, UserId, ToRecipientType(Recipient));

                public override TResult Apply<TResult>(
                    Func<EndUser, TResult> endUser,
                    Func<Distributor, TResult> distributor,
                    Func<Service, TResult> service
                ) => distributor(this);

                public enum RecipientType
                {
                    Everyone,
                    Service
                }
            }

            [JsonDiscriminator("service")]
            public sealed class Service : CaseCommentAuthor
            {
                public Service(SystemUserId systemUserId, RecipientType recipient)
                {
                    SystemUserId = systemUserId;
                    Recipient = recipient;
                }

                public SystemUserId SystemUserId { get; }
                public RecipientType Recipient { get; }

                public static Service Create(Cases.CaseCommentAuthor.Service source) =>
                    new Service(source.SystemUserId, ToRecipientType(source.Recipient));

                private static RecipientType ToRecipientType(Cases.CaseCommentAuthor.Service.RecipientType source) =>
                    source switch
                    {
                        Cases.CaseCommentAuthor.Service.RecipientType.Everyone => RecipientType.Everyone,
                        Cases.CaseCommentAuthor.Service.RecipientType.Distributor => RecipientType.Distributor,
                        _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
                    };

                private static Cases.CaseCommentAuthor.Service.RecipientType ToRecipientType(RecipientType source) =>
                    source switch
                    {
                        RecipientType.Everyone => Cases.CaseCommentAuthor.Service.RecipientType.Everyone,
                        RecipientType.Distributor => Cases.CaseCommentAuthor.Service.RecipientType.Distributor,
                        _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
                    };

                public override Cases.CaseCommentAuthor ToCaseCommentAuthor() =>
                    new Cases.CaseCommentAuthor.Service(SystemUserId, ToRecipientType(Recipient));

                public override TResult Apply<TResult>(
                    Func<EndUser, TResult> endUser,
                    Func<Distributor, TResult> distributor,
                    Func<Service, TResult> service
                ) => service(this);

                public enum RecipientType
                {
                    Everyone,
                    Distributor
                }
            }
        }
    }
}