using System;
using System.Threading.Tasks;
using EventSourcing.Demo.Cases.Events;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public sealed partial class Case : Aggregate<CaseId, Case>,
        IApplies<CaseImported>,
        IApplies<CaseAssignedToDistributor>,
        IApplies<CaseAssignedToService>
    {
        public string Subject { get; private set; }
        public string Description { get; private set; }

        public string? CaseNumber { get; private set; }

        public CaseStatus Status { get; private set; }

        private Case(CaseId id, CaseCreated @event)
            : base(id)
        {
            Subject = @event.Subject;
            Description = @event.Description;

            CaseNumber = null;

            Status = CaseStatus.InProgress;
        }

        private Case(CaseId id, CaseImported @event)
            : base(id)
        {
            Subject = @event.Subject;
            Description = @event.Description;

            CaseNumber = @event.CaseNumber;

            Status = @event.Status;
        }

        public void Import(string subject, string description, string caseNumber, CaseStatus status)
        {
            var @event = new CaseImported(subject, description, caseNumber, status);

            Apply(@event);
            Append(Guid.NewGuid(), CaseImported.EventType, @event);
        }

        public void AssignToDistributor()
        {
            var @event = new CaseAssignedToDistributor();

            Apply(@event);
            Append(Guid.NewGuid(), CaseAssignedToDistributor.EventType, @event);
        }

        public void AssignToService()
        {
            var @event = new CaseAssignedToService();

            Apply(@event);
            Append(Guid.NewGuid(), CaseAssignedToService.EventType, @event);
        }

        public void Apply(CaseImported @event)
        {
            Subject = @event.Subject;
            Description = @event.Description;

            CaseNumber = @event.CaseNumber;

            Status = @event.Status;
        }

        public void Apply(CaseAssignedToDistributor @event)
        {
            Status = CaseStatus.WaitingForDistributor;
        }

        public void Apply(CaseAssignedToService @event)
        {
            Status = CaseStatus.InProgress;
        }
    }
}