using Aggregail.MongoDB.Admin.Hubs;

namespace Aggregail.MongoDB.Admin.Controllers
{
    public sealed class StreamResponse
    {
        public StreamResponse(RecordedEvent[] events)
        {
            Events = events;
        }

        public RecordedEvent[] Events { get; }
    }
}