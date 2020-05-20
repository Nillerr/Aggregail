using System;
using System.Threading;
using System.Threading.Tasks;
using Aggregail.MongoDB.Admin.Documents;
using Aggregail.Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aggregail.MongoDB.Admin.Hubs
{
    public sealed class StreamService : BackgroundService
    {
        private readonly JsonEventSerializer _serializer = new JsonEventSerializer(JsonSerializer.CreateDefault());
        
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
                    var recordedEvent = csd.FullDocument;
                    var operationType = csd.OperationType;
                    await BroadcastOperationAsync(operationType, recordedEvent);
                },
                cancellationToken
            );
        }

        private async Task BroadcastOperationAsync(
            ChangeStreamOperationType operationType,
            RecordedEventDocument document
        )
        {
            switch (operationType)
            {
                case ChangeStreamOperationType.Insert:
                {
                    var recordedEvent = ToRecordedEvent(document);
                    Console.WriteLine("Broadcasting insert:");
                    Console.WriteLine(recordedEvent.Data.ToString(Formatting.Indented));
                    await _hub.Clients.All.EventRecorded(recordedEvent);
                    break;
                }
                case ChangeStreamOperationType.Update:
                case ChangeStreamOperationType.Replace:
                case ChangeStreamOperationType.Delete:
                case ChangeStreamOperationType.Invalidate:
                case ChangeStreamOperationType.Rename:
                case ChangeStreamOperationType.Drop:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null);
            }
        }

        private RecordedEvent ToRecordedEvent(RecordedEventDocument document)
        {
            var data = _serializer.Deserialize<JObject>(document.Data);

            var recordedEvent = new RecordedEvent
            {
                Id = document.Id,
                Stream = document.Stream,
                EventId = document.EventId,
                EventType = document.EventType,
                EventNumber = document.EventNumber,
                Created = document.Created,
                Data = data
            };
            
            return recordedEvent;
        }
    }
}