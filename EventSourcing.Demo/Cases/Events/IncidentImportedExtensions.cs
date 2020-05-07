using System;

namespace EventSourcing.Demo.Cases.Events
{
    public static class IncidentImportedExtensions
    {
        public static CaseType ImportedCaseType(this IncidentImported source)
        {
            switch (source.Type)
            {
                case IncidentImported.CaseType.Claims:
                    if (source.RobotId == null)
                        throw new InvalidOperationException();

                    return new CaseType.Service(new CaseRobot(source.RobotId.Value));
                
                case IncidentImported.CaseType.Service:
                    return source.RobotId.HasValue
                        ? new CaseType.Service(new CaseRobot(source.RobotId.Value))
                        : new CaseType.Service();
                
                case IncidentImported.CaseType.Support:
                    return source.RobotId.HasValue
                        ? new CaseType.Support(new CaseRobot(source.RobotId.Value))
                        : new CaseType.Support();
                
                default:
                    throw new InvalidOperationException($"The case type {source.Type} was unregonized");
            }
        }

        public static CaseStatus ImportedCaseStatus(this IncidentImported source)
        {
            switch (source.Status)
            {
                case IncidentImported.CaseStatus.InProgress:
                    return CaseStatus.InProgress;
                
                case IncidentImported.CaseStatus.OnHold:
                case IncidentImported.CaseStatus.WaitingForDetails:
                case IncidentImported.CaseStatus.Researching:
                case IncidentImported.CaseStatus.Canceled:
                case IncidentImported.CaseStatus.InformationProvided:
                case IncidentImported.CaseStatus.Merged:
                case IncidentImported.CaseStatus.WaitingReplyService:
                case IncidentImported.CaseStatus.WaitingReplyCts:
                case IncidentImported.CaseStatus.WaitingReplyRD:
                case IncidentImported.CaseStatus.WaitingReplyCustomer:
                case IncidentImported.CaseStatus.WaitingForEstimate:
                    throw new InvalidOperationException();
                
                case IncidentImported.CaseStatus.WaitingForDistributor:
                    return CaseStatus.WaitingForDistributor;
                
                case IncidentImported.CaseStatus.ProblemSolved:
                case IncidentImported.CaseStatus.ResolvedByDistributor:
                    return CaseStatus.Resolved;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }
    }
}