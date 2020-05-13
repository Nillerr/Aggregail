using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using EventSourcing.Demo.Cases.CRM;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases.Events
{
    public sealed class IncidentImported
    {
        public static EventType<IncidentImported> EventType = "IncidentImported";

        public IncidentImported(
            string title,
            string description,
            CaseNumber caseNumber,
            CaseType type,
            CaseOriginCode origin,
            CaseStatus status,
            RobotId? robotId,
            RobotApplication? application,
            SoftwareVersionId? softwareVersionId,
            ImmutableList<RobotErrorCode> errorCodes
        )
        {
            Title = title;
            Description = description;
            CaseNumber = caseNumber;
            Type = type;
            Origin = origin;
            Status = status;
            RobotId = robotId;
            Application = application;
            SoftwareVersionId = softwareVersionId;
            ErrorCodes = errorCodes;
        }

        public string Title { get; }
        public string Description { get; }

        public CaseNumber CaseNumber { get; }

        public CaseType Type { get; }
        public CaseOriginCode Origin { get; }
        public CaseStatus Status { get; }

        // Robot Stuff
        public RobotId? RobotId { get; }

        public RobotApplication? Application { get; }

        public SoftwareVersionId? SoftwareVersionId { get; }

        public ImmutableList<RobotErrorCode> ErrorCodes { get; }

        public static IncidentImported Create(Incident source)
        {
            return new IncidentImported(
                source.Title,
                source.Description,
                new CaseNumber(source.CaseNumber),
                ToCaseType(source.Type),
                ToCaseOriginCode(source.Origin),
                ToCaseStatus(source.Status),
                source.RobotId.Select(Cases.RobotId.Create),
                source.Application.Select(ToRobotApplication),
                source.SoftwareVersionId.Select(Cases.SoftwareVersionId.Create),
                source.ErrorCodes.Select(RobotErrorCode.Create).ToImmutableList()
            );
        }

        public enum CaseType
        {
            Claims,
            Support,
            Service,
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum CaseOriginCode
        {
            UREmployee = 315_810_002,
            PartnerPortalCreated = 315_810_000,
            CustomerPortalCreated = 315_810_001,
            CustomerEmail = 229_280_001,
            CustomerCall = 1,
            PartnerEmail = 2,
            PartnerCall = 315_810_003,
            Other = 229_280_000
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum CaseStatus
        {
            InProgress = 1,
            OnHold = 2,
            WaitingForDetails = 3,
            Researching = 4,
            ProblemSolved = 5,
            Canceled = 6,

            InformationProvided = 1000,
            Merged = 2000,

            WaitingReplyService = 100_000_000,
            WaitingReplyCts = 100_000_001,
            WaitingReplyRD = 100_000_002,
            WaitingReplyCustomer = 100_000_003,
            WaitingForEstimate = 100_000_004,

            ResolvedByDistributor = 315_810_000,
            WaitingForDistributor = 315_810_001,
        }

        public enum RobotApplication
        {
            Assembly,
            MaterialHandling,
            MachineTending,
            MaterialRemoval,
            Quality,
            Welding,
            Other,
            Finishing,
            Dispensing,
        }

        private static CaseType ToCaseType(CRM.CaseType source)
        {
            return source switch
            {
                CRM.CaseType.Claims => CaseType.Claims,
                CRM.CaseType.Support => CaseType.Support,
                CRM.CaseType.Service => CaseType.Service,
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }

        private static CaseOriginCode ToCaseOriginCode(CRM.CaseOriginCode source)
        {
            return source switch
            {
                CRM.CaseOriginCode.UREmployee => CaseOriginCode.UREmployee,
                CRM.CaseOriginCode.PartnerPortalCreated => CaseOriginCode.PartnerPortalCreated,
                CRM.CaseOriginCode.CustomerPortalCreated => CaseOriginCode.CustomerPortalCreated,
                CRM.CaseOriginCode.CustomerEmail => CaseOriginCode.CustomerEmail,
                CRM.CaseOriginCode.CustomerCall => CaseOriginCode.CustomerCall,
                CRM.CaseOriginCode.PartnerEmail => CaseOriginCode.PartnerEmail,
                CRM.CaseOriginCode.PartnerCall => CaseOriginCode.PartnerCall,
                CRM.CaseOriginCode.Other => CaseOriginCode.Other,
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }

        private static CaseStatus ToCaseStatus(CRM.CaseStatus source)
        {
            return source switch
            {
                CRM.CaseStatus.InProgress => CaseStatus.InProgress,
                CRM.CaseStatus.OnHold => CaseStatus.OnHold,
                CRM.CaseStatus.WaitingForDetails => CaseStatus.WaitingForDetails,
                CRM.CaseStatus.Researching => CaseStatus.Researching,
                CRM.CaseStatus.ProblemSolved => CaseStatus.ProblemSolved,
                CRM.CaseStatus.Canceled => CaseStatus.Canceled,
                CRM.CaseStatus.InformationProvided => CaseStatus.InformationProvided,
                CRM.CaseStatus.Merged => CaseStatus.Merged,
                CRM.CaseStatus.WaitingReplyService => CaseStatus.WaitingReplyService,
                CRM.CaseStatus.WaitingReplyCts => CaseStatus.WaitingReplyCts,
                CRM.CaseStatus.WaitingReplyRD => CaseStatus.WaitingReplyRD,
                CRM.CaseStatus.WaitingReplyCustomer => CaseStatus.WaitingReplyCustomer,
                CRM.CaseStatus.WaitingForEstimate => CaseStatus.WaitingForEstimate,
                CRM.CaseStatus.ResolvedByDistributor => CaseStatus.ResolvedByDistributor,
                CRM.CaseStatus.WaitingForDistributor => CaseStatus.WaitingForDistributor,
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }

        private static RobotApplication ToRobotApplication(CRM.RobotApplication source)
        {
            return source switch
            {
                CRM.RobotApplication.Assembly => RobotApplication.Assembly,
                CRM.RobotApplication.MaterialHandling => RobotApplication.MaterialHandling,
                CRM.RobotApplication.MachineTending => RobotApplication.MachineTending,
                CRM.RobotApplication.MaterialRemoval => RobotApplication.MaterialRemoval,
                CRM.RobotApplication.Quality => RobotApplication.Quality,
                CRM.RobotApplication.Welding => RobotApplication.Welding,
                CRM.RobotApplication.Other => RobotApplication.Other,
                CRM.RobotApplication.Finishing => RobotApplication.Finishing,
                CRM.RobotApplication.Dispensing => RobotApplication.Dispensing,
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }
    }
}