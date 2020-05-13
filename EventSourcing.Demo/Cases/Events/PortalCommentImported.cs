using System;
using System.Collections.Immutable;
using EventSourcing.Demo.Cases.CRM;
using EventSourcing.Demo.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EventSourcing.Demo.Cases.Events
{
    public sealed class PortalCommentImported
    {
        public static readonly EventType<PortalCommentImported> EventType = "PortalCommentImported";

        public PortalCommentImported(
            PortalCommentId id,
            string description,
            SystemUserId ownerId,
            PortalCommentDirectionCode? directionCode,
            PortalCommentAudience audience,
            ImmutableList<ActivityParty> activityParties
        )
        {
            Id = id;
            Description = description;
            OwnerId = ownerId;
            DirectionCode = directionCode;
            Audience = audience;
            ActivityParties = activityParties;
        }

        public static PortalCommentImported Create(PortalComment source)
        {
            return new PortalCommentImported(
                new PortalCommentId(source.Id),
                source.Description,
                new SystemUserId(source.OwnerId),
                source.DirectionCode.Select(ToPortalCommentDirectionCode),
                ToPortalCommentAudience(source.Audience),
                source.
            );
        }

        private static PortalCommentAudience ToPortalCommentAudience(CRM.PortalCommentAudience source) =>
            source switch
            {
                CRM.PortalCommentAudience.EndUser => PortalCommentAudience.EndUser,
                CRM.PortalCommentAudience.Distributor => PortalCommentAudience.Distributor,
                CRM.PortalCommentAudience.SubDistributorOrSI => PortalCommentAudience.SubDistributorOrSI,
                CRM.PortalCommentAudience.DistributorAndSI => PortalCommentAudience.DistributorAndSI,
                CRM.PortalCommentAudience.All => PortalCommentAudience.All,
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };

        private static PortalCommentDirectionCode ToPortalCommentDirectionCode(CRM.PortalCommentDirectionCode source) =>
            source switch
            {
                CRM.PortalCommentDirectionCode.None => PortalCommentDirectionCode.None,
                CRM.PortalCommentDirectionCode.Incoming => PortalCommentDirectionCode.Incoming,
                CRM.PortalCommentDirectionCode.Outgoing => PortalCommentDirectionCode.Outgoing,
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };

        public PortalCommentId Id { get; }
        public string Description { get; }
        public SystemUserId OwnerId { get; }
        public PortalCommentDirectionCode? DirectionCode { get; }
        public PortalCommentAudience Audience { get; }
        public ImmutableList<ActivityParty> ActivityParties { get; }

        public sealed class ActivityParty
        {
            public ActivityParty(Guid partyId, ParticipationTypeMask type)
            {
                PartyId = partyId;
                Type = type;
            }

            public Guid PartyId { get; }
            public ParticipationTypeMask Type { get; }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum ParticipationTypeMask
        {
            Sender,
            ToRecipient,
            Owner,
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum PortalCommentAudience
        {
            EndUser,
            Distributor,
            SubDistributorOrSI,
            DistributorAndSI,
            All,
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum PortalCommentDirectionCode
        {
            None,
            Incoming,
            Outgoing,
        }
    }
}