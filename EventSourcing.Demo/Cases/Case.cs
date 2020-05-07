using System;
using System.Threading.Tasks;
using EventSourcing.Demo.Cases.Events;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public sealed class Case : Aggregate<CaseId, Case>,
        IApplies<IncidentImported>,
        IApplies<CaseAssignedToDistributor>,
        IApplies<CaseAssignedToService>
    {
        private static readonly AggregateConfiguration<CaseId, Case> Configuration =
            new AggregateConfiguration<CaseId, Case>("case")
                .Constructs(CaseCreated.EventType, (id, e) => new Case(id, e))
                .Applies(IncidentImported.EventType)
                .Applies(CaseAssignedToDistributor.EventType)
                .Applies(CaseAssignedToService.EventType);

        public static Case Create(CaseId id, string subject, string description, CaseType type)
        {
            var e = new CaseCreated(subject, description, type);

            var @case = new Case(id, e);
            @case.Append(id.Value, CaseCreated.EventType, e);
            return @case;
        }

        public static Task<Case?> FromAsync(IEventStoreReader reader, CaseId id)
        {
            return reader.AggregateAsync(id, Configuration);
        }

        private Case(CaseId id, CaseCreated e)
            : base(id)
        {
            Subject = e.Subject;
            Description = e.Description;

            CaseNumber = null;

            Status = CaseStatus.InProgress;

            Type = e.Type;
        }

        public string Subject { get; private set; }
        public string Description { get; private set; }

        public CaseNumber? CaseNumber { get; private set; }

        public CaseStatus Status { get; private set; }
        
        public CaseType Type { get; private set; }

        public Task CommitAsync(IEventStoreAppender appender)
        {
            return CommitAsync(appender, Configuration);
        }

        public void Import(CRM.Incident incident)
        {
            var e = IncidentImported.Create(incident);

            Apply(e);
            Append(Guid.NewGuid(), IncidentImported.EventType, e);
        }

        public void AssignToDistributor()
        {
            var e = new CaseAssignedToDistributor();

            Apply(e);
            Append(Guid.NewGuid(), CaseAssignedToDistributor.EventType, e);
        }

        public void AssignToService()
        {
            var e = new CaseAssignedToService();

            Apply(e);
            Append(Guid.NewGuid(), CaseAssignedToService.EventType, e);
        }

        public void Apply(IncidentImported e)
        {
            Subject = e.Title;
            Description = e.Description;

            CaseNumber = e.CaseNumber;

            Status = e.ImportedCaseStatus();

            Type = e.ImportedCaseType();
        }

        public void Apply(CaseAssignedToDistributor e)
        {
            Status = CaseStatus.WaitingForDistributor;
        }

        public void Apply(CaseAssignedToService e)
        {
            Status = CaseStatus.InProgress;
        }
    }
}