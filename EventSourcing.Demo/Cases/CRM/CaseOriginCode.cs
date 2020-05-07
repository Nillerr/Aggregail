using System.Diagnostics.CodeAnalysis;

namespace EventSourcing.Demo.Cases.CRM
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum CaseOriginCode
    {
        UREmployee = 315810002,
        PartnerPortalCreated = 315810000,
        CustomerPortalCreated = 315810001,
        CustomerEmail = 229280001,
        CustomerCall = 1,
        PartnerEmail = 2,
        PartnerCall = 315810003,
        Other = 229280000
    }
}