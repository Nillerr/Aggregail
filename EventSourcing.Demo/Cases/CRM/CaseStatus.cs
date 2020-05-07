using System.Diagnostics.CodeAnalysis;

namespace EventSourcing.Demo.Cases.CRM
{
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
}