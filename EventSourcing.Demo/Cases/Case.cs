using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using EventSourcing.Demo.Cases.Events;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public sealed partial class Case : Aggregate<CaseId, Case>,
        IApplies<IncidentImported>,
        IApplies<CaseAssignedToDistributor>,
        IApplies<CaseAssignedToService>,
        IApplies<CaseCommentCreated>,
        IApplies<PortalCommentImported>,
        IApplies<CaseCommentExported>,
        IApplies<PortalCommentAnnotationImported>
    {
        private static readonly AggregateConfiguration<CaseId, Case> Configuration =
            new AggregateConfiguration<CaseId, Case>("case")
                .Constructs(CaseCreated.EventType, (id, e) => new Case(id, e))
                .Applies(IncidentImported.EventType)
                .Applies(CaseAssignedToDistributor.EventType)
                .Applies(CaseAssignedToService.EventType)
                .Applies(CaseCommentCreated.EventType)
                .Applies(PortalCommentImported.EventType)
                .Applies(CaseCommentExported.EventType)
                .Applies(PortalCommentAnnotationImported.EventType);

        public static Case Create(
            CaseId id,
            string subject,
            string description,
            CaseType type,
            params CaseAttachmentId[] attachmentIds
        )
        {
            var e = new CaseCreated(subject, description, type, attachmentIds.ToImmutableList());

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
            
            Attachments = e.AttachmentIds
                .Select(a => new CaseAttachment(a))
                .ToImmutableList();
            
            Comments = ImmutableList<CaseComment>.Empty;
        }

        public string Subject { get; private set; }
        public string Description { get; private set; }

        public CaseNumber? CaseNumber { get; private set; }

        public CaseStatus Status { get; private set; }
        
        public CaseType Type { get; private set; }
        
        public ImmutableList<CaseAttachment> Attachments { get; private set; }
        
        public ImmutableList<CaseComment> Comments { get; private set; }

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

        public void CreateComment(
            CaseCommentId id,
            string description,
            CaseCommentAuthor author,
            params CaseCommentAttachmentId[] attachmentIds
        )
        {
            var e = CaseCommentCreated.Create(id, description, author, attachmentIds);
            
            Apply(e);
            Append(Guid.NewGuid(), CaseCommentCreated.EventType, e);
        }

        public void ImportPortalComment(CRM.PortalComment source)
        {
            var e = PortalCommentImported.Create(source);
            
            Apply(e);
            Append(Guid.NewGuid(), PortalCommentImported.EventType, e);
        }

        public void ExportComment(CaseCommentId id)
        {
            var e = new CaseCommentExported(id);
            
            Apply(e);
            Append(Guid.NewGuid(), CaseCommentExported.EventType, e);
        }

        public void ImportPortalCommentAnnotation(AnnotationId id, PortalCommentId portalCommentId)
        {
            var e = new PortalCommentAnnotationImported(id, portalCommentId);
            
            Apply(e);
            Append(Guid.NewGuid(), PortalCommentAnnotationImported.EventType, e);
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

        public void Apply(CaseCommentCreated e)
        {
            var comment = new CaseComment(e);
            Comments = Comments.Add(comment);
        }

        public void Apply(PortalCommentImported e)
        {
            var existing = Comments.FirstOrDefault(c => c.Id == e.Id);
            if (existing == null)
            {
                var comment = new CaseComment(e);
                Comments = Comments.Add(comment);
            }
            else
            {
                existing.Apply(e);
            }
        }

        public void Apply(CaseCommentExported e)
        {
            var comment = Comments.First(c => c.Id == e.Id);
            comment.Apply(e);
        }

        public void Apply(PortalCommentAnnotationImported e)
        {
            var comment = Comments.First(c => c.Id == e.PortalCommentId);
            comment.Apply(e);
        }
    }
}