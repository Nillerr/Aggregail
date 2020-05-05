using System;
using System.Threading.Tasks;
using EventSourcing.Demo.Cases.Events;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public sealed class Case : Aggregate<Case>,
        IApplies<CaseImported>,
        IApplies<CaseAssignedToDistributor>,
        IApplies<CaseAssignedToService>
    {
        private static readonly AggregateConfiguration<Case> Configuration =
            new AggregateConfiguration<Case>("Case")
                .Constructs(CaseCreated.EventType, (id, @event) => new Case(id, @event))
                .Constructs(CaseImported.EventType, (id, @event) => new Case(id, @event))
                .Applies(CaseImported.EventType)
                .Applies(CaseAssignedToDistributor.EventType)
                .Applies(CaseAssignedToService.EventType);

        public string Subject { get; private set; }
        public string Description { get; private set; }

        public string? CaseNumber { get; private set; }

        public CaseStatus Status { get; private set; }

        private Case(Guid id, CaseCreated @event)
            : base(id)
        {
            Subject = @event.Subject;
            Description = @event.Description;

            CaseNumber = null;

            Status = CaseStatus.InProgress;
        }

        private Case(Guid id, CaseImported @event)
            : base(id)
        {
            Subject = @event.Subject;
            Description = @event.Description;

            CaseNumber = @event.CaseNumber;

            Status = @event.Status;
        }

        public static Case Create(Guid id, string subject, string description)
        {
            var @event = new CaseCreated(subject, description);

            var @case = new Case(id, @event);
            @case.Append(id, CaseCreated.EventType, @event);
            return @case;
        }

        public static Case Imported(Guid id, string subject, string description, string caseNumber, CaseStatus status)
        {
            var @event = new CaseImported(subject, description, caseNumber, status);

            var @case = new Case(id, @event);
            @case.Append(id, CaseImported.EventType, @event);
            return @case;
        }

        public static Task<Case?> FromAsync(IEventStoreReader reader, Guid id)
        {
            return reader.AggregateAsync(id, Configuration);
        }

        public Task CommitAsync(IEventStoreAppender appender)
        {
            return CommitAsync(appender, Configuration);
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