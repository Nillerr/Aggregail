using System;
using System.Threading;
using System.Threading.Tasks;
using Aggregail.MongoDB.Admin.Documents;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Aggregail.MongoDB.Admin.Hubs
{
    public sealed class StreamService : BackgroundService
    {
        private readonly IMongoCollection<RecordedEventDocument> _events;
        private readonly IHubContext<StreamHub, IStreamClient> _hub;

        public StreamService(IMongoCollection<RecordedEventDocument> events, IHubContext<StreamHub, IStreamClient> hub)
        {
            _events = events;
            _hub = hub;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var pipelineDefinition = PipelineDefinitionBuilder.For<ChangeStreamDocument<RecordedEventDocument>>();

            var cursor = await _events.WatchAsync(pipelineDefinition, cancellationToken: cancellationToken);
            await cursor.ForEachAsync(async csd =>
                {
                    var json = csd.BackingDocument.ToJson();
                    var recordedEvent = JsonConvert.DeserializeObject<RecordedEvent>(json);
                    switch (csd.OperationType)
                    {
                        case ChangeStreamOperationType.Insert:
                            await _hub.Clients.All.EventRecorded(recordedEvent);
                            break;
                        case ChangeStreamOperationType.Update:
                            break;
                        case ChangeStreamOperationType.Replace:
                            break;
                        case ChangeStreamOperationType.Delete:
                            break;
                        case ChangeStreamOperationType.Invalidate:
                            break;
                        case ChangeStreamOperationType.Rename:
                            break;
                        case ChangeStreamOperationType.Drop:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                },
                cancellationToken
            );
        }
    }
}