using System;

namespace EventSourcing.Demo.Cases
{
    public static class CaseStatusExtensions
    {
        public static CaseStatus Imported(this CRM.CaseStatus source)
        {
            switch (source)
            {
                case CRM.CaseStatus.InProgress:
                    return CaseStatus.InProgress;
                
                case CRM.CaseStatus.OnHold:
                case CRM.CaseStatus.WaitingForDetails:
                case CRM.CaseStatus.Researching:
                case CRM.CaseStatus.Canceled:
                case CRM.CaseStatus.InformationProvided:
                case CRM.CaseStatus.Merged:
                case CRM.CaseStatus.WaitingReplyService:
                case CRM.CaseStatus.WaitingReplyCts:
                case CRM.CaseStatus.WaitingReplyRD:
                case CRM.CaseStatus.WaitingReplyCustomer:
                case CRM.CaseStatus.WaitingForEstimate:
                    throw new InvalidOperationException();
                
                case CRM.CaseStatus.WaitingForDistributor:
                    return CaseStatus.WaitingForDistributor;
                
                case CRM.CaseStatus.ProblemSolved:
                case CRM.CaseStatus.ResolvedByDistributor:
                    return CaseStatus.Resolved;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }
    }
}