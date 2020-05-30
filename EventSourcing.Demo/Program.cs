using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aggregail;
using Aggregail.MongoDB;
using Aggregail.Newtonsoft.Json;
using EventSourcing.Demo.Cases;
using EventSourcing.Demo.Robots;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace EventSourcing.Demo
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                Converters =
                {
                    new StringEnumConverter()
                }
            };
            
            var jsonSerializer = JsonSerializer.CreateDefault(jsonSerializerSettings);
            var serializer = new JsonEventSerializer(jsonSerializer);
            // var serializer = new Aggregail.System.Text.Json.JsonEventSerializer(new JsonSerializerOptions());

            // using var connection = EventStoreConnection.Create("ConnectTo=tcp://admin:changeit@localhost:1113");
            // await connection.ConnectAsync();
            // var eventStore = new EventStore(connection, serializer);

            var mongoClient = new MongoClient("mongodb://root:example@mongodb-primary:27017,mongodb-secondary:27018,mongodb-arbiter:27019/aggregail_demo?authSource=admin&replicaSet=rs0");
            // var mongoClient = new MongoClient("mongodb://root:example@mongodb:27017/aggregail_demo?authSource=admin&replicaSet=rs0");
            var mongoDatabase = mongoClient.GetDatabase("aggregail_demo");
            
            var mongoSettings = new MongoEventStoreSettings(mongoDatabase, "streams", serializer);
            mongoSettings.MetadataFactory = new MetadataFactory("nije");
            
            var mongoStore = new MongoEventStore(mongoSettings);

            await mongoStore.InitializeAsync();

            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("Running Test Case...");

            await RobotProgram.RunAsync(mongoStore);
            
            // var tasks = Enumerable.Range(0, 500)
            //     .Select(_ => TestCase(mongoStore))
            //     .ToArray();
            //
            // await Task.WhenAll(tasks);
            sw.Stop();
            
            Console.WriteLine($"Test Case Complete in {sw.Elapsed}");
        }

        private static async Task TestCase(IEventStore store)
        {
            var id = new CaseId(Guid.NewGuid());

            await CreateCaseAsync(store, id);
            await ModifyCaseAsync(store, id);

            var @case = await Case.FromAsync(store, id);
            Console.WriteLine(JsonConvert.SerializeObject(@case, Formatting.Indented));
        }

        private static async Task CreateCaseAsync(IEventStore store, CaseId id)
        {
            // Case-ec4f433b-f7e0-41b8-93cc-338d373214ab
            var @case = Case.Create(id, "The Subject", "The Description");
            await @case.CommitAsync(store);
        }

        private static async Task ModifyCaseAsync(IEventStore store, CaseId id)
        {
            var @case = await Case.FromAsync(store, id);
            if (@case == null)
            {
                throw new InvalidOperationException($"The case with id {id} does not exist");
            }

            @case.Import("Imported Subject", "Imported Description", "TS012345", CaseStatus.WaitingForDistributor);
            @case.AssignToService();

            await @case.CommitAsync(store);
        }
    }
}