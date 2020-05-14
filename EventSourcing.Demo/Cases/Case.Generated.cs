#nullable enable
using System.Threading.Tasks;
using EventSourcing.Demo.Cases.Events;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public partial class Case
    {
        private static readonly AggregateConfiguration<CaseId, Case> Configuration =
            new AggregateConfiguration<CaseId, Case>("Case")
                .Constructs(CaseCreated.EventType, (id, @event) => new Case(id, @event))
                .Constructs(CaseImported.EventType, (id, @event) => new Case(id, @event))
                .Applies(CaseImported.EventType)
                .Applies(CaseAssignedToDistributor.EventType)
                .Applies(CaseAssignedToService.EventType);

        public static Case Create(CaseId id, string subject, string description)
        {
            var @event = new CaseCreated(subject, description);

            var @case = new Case(id, @event);
            @case.Append(id.Value, CaseCreated.EventType, @event);
            return @case;
        }

        public static Case Imported(CaseId id, string subject, string description, string caseNumber, CaseStatus status)
        {
            var @event = new CaseImported(subject, description, caseNumber, status);

            var @case = new Case(id, @event);
            @case.Append(id.Value, CaseImported.EventType, @event);
            return @case;
        }

        public static Task<Case?> FromAsync(IEventStoreReader reader, CaseId id)
        {
            return reader.AggregateAsync(id, Configuration);
        }

        public Task CommitAsync(IEventStoreAppender appender)
        {
            return CommitAsync(appender, Configuration);
        }
    }
}