using System.Collections.Immutable;
using System.Linq;
using EventSourcing.Demo.Cases.Events;

namespace EventSourcing.Demo.Cases
{
    public sealed class CaseComment
    {
        public CaseComment(CaseCommentCreated e)
        {
            Id = e.Id;
            Description = e.Description;
            Author = e.Author.Apply<CaseCommentAuthor>(
                endUser => new CaseCommentAuthor.EndUser(
                    endUser.Id,
                    endUser.UserId
                ),
                distributor => new CaseCommentAuthor.Distributor(
                    distributor.Id,
                    distributor.UserId,
                    (CaseCommentAuthor.Distributor.RecipientType) distributor.Recipient
                ),
                service => new CaseCommentAuthor.Service(
                    service.SystemUserId,
                    (CaseCommentAuthor.Service.RecipientType) service.Recipient
                )
            );
            Attachments = ImmutableList<CaseCommentAttachment>.Empty;
        }

        public CaseComment(PortalCommentImported e)
        {
            Id = new CaseCommentId(e.Id.Value);
            Description = e.Description;
            Author = CaseCommentAuthor.Service.Create(e);
            Attachments = ImmutableList<CaseCommentAttachment>.Empty;
        }

        public CaseCommentId Id { get; }
        public string Description { get; private set; }
        public CaseCommentAuthor Author { get; private set; }
        
        public ImmutableList<CaseCommentAttachment> Attachments { get; private set; }

        public void Apply(PortalCommentImported e)
        {
            Description = e.Description;
            // TODO @nsj:
            // Author = e.;
        }

        public void Apply(CaseCommentExported e)
        {
            // Nothing
        }

        public void Apply(PortalCommentAnnotationImported e)
        {
            var existing = Attachments.FirstOrDefault(a => a.Id == e.Id);
            if (existing == null)
            {
                var attachment = new CaseCommentAttachment(e);
                Attachments = Attachments.Add(attachment);
            }
            else
            {
                existing.Apply(e);
            }
        }
    }
}