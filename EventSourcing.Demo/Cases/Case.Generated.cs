#nullable enable
using System.Threading.Tasks;
using Aggregail;
using EventSourcing.Demo.Cases.Events;

namespace EventSourcing.Demo.Cases
{
    public partial class Case
    {
        private static readonly AggregateConfiguration<CaseId, Case> Configuration =
            new AggregateConfiguration<CaseId, Case>("Case", CaseId.Parse)
                .Constructs(CaseCreated.EventType, (id, @event) => new Case(id, @event))
                .Constructs(CaseImported.EventType, (id, @event) => new Case(id, @event))
                .Applies(CaseImported.EventType, (agg, e) => agg.Apply(e))
                .Applies(CaseAssignedToDistributor.EventType, (agg, e) => agg.Apply(e))
                .Applies(CaseAssignedToService.EventType, (agg, e) => agg.Apply(e));

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

        public static Task<Case?> FromAsync(IEventStore store, CaseId id)
        {
            return store.AggregateAsync(id, Configuration);
        }

        public Task CommitAsync(IEventStore store)
        {
            return CommitAsync(store, Configuration);
        }
    }
}