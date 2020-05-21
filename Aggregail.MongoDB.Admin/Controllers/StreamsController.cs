using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aggregail.MongoDB.Admin.Documents;
using Aggregail.MongoDB.Admin.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Aggregail.MongoDB.Admin.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/streams")]
    public sealed class StreamsController : ControllerBase
    {
        private readonly IMongoCollection<RecordedEventDocument> _events;

        public StreamsController(IMongoCollection<RecordedEventDocument> events)
        {
            _events = events;
        }

        [HttpGet]
        public async Task<IActionResult> RecentStreamsAsync(CancellationToken cancellationToken)
        {
            var recentlyCreatedDocuments = await _events
                .Find(e => e.EventNumber == 0)
                .SortByDescending(e => e.Created)
                .Limit(100)
                .Project(e => new {Name = e.Stream})
                .ToListAsync(cancellationToken);

            var recentlyCreatedStreams = recentlyCreatedDocuments
                .Select(stream => new Stream(stream.Name))
                .Distinct()
                .ToList();

            var recentlyChangedDocuments = await _events
                .Find(FilterDefinition<RecordedEventDocument>.Empty)
                .SortByDescending(e => e.Created)
                .Limit(100)
                .Project(e => new {Name = e.Stream})
                .ToListAsync(cancellationToken);

            var recentlyChangedStreams = recentlyChangedDocuments
                .Select(stream => new Stream(stream.Name))
                .Distinct()
                .ToList();

            return Ok(new
                {
                    RecentlyCreatedStreams = recentlyCreatedStreams,
                    RecentlyChangedStreams = recentlyChangedStreams,
                }
            );
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<StreamResponse>> StreamAsync(string name, CancellationToken cancellationToken)
        {
            var response = await ReadStreamEventsForwardAsync(name, 0, 20, cancellationToken);
            if (response.Events.Length == 0)
            {
                return NotFound();
            }
            
            return response;
        }

        [HttpGet("{name}/{eventNumber}")]
        public async Task<IActionResult> StreamEventAsync(
            string name,
            int eventNumber,
            CancellationToken cancellationToken
        )
        {
            var document = await _events
                .Find(e => e.Stream == name && e.EventNumber == eventNumber)
                .FirstOrDefaultAsync(cancellationToken);

            if (document == null)
            {
                return NotFound();
            }

            var @event = RecordedEvent.FromDocument(document);
            return Ok(@event);
        }

        [HttpGet("{name}/{from}/forward/{limit}")]
        public async Task<StreamResponse> ReadStreamEventsForwardAsync(
            string name,
            int from,
            int limit,
            CancellationToken cancellationToken
        )
        {
            var documents = await _events
                .Find(e => e.Stream == name && e.EventNumber >= from)
                .SortBy(e => e.EventNumber)
                .Limit(limit)
                .ToListAsync(cancellationToken);

            var events = documents
                .Select(RecordedEvent.FromDocument)
                .ToArray();

            return new StreamResponse(events);
        }
    }
}