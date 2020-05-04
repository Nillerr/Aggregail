using System;
using System.Threading.Tasks;
using EventSourcing.Demo.Cases;
using EventSourcing.Demo.Framework;
using EventSourcing.Demo.Framework.Serialiazation;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EventSourcing.Demo
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using var connection = EventStoreConnection.Create("ConnectTo=tcp://admin:changeit@localhost:1113");
            
            await connection.ConnectAsync();
            
            var encoder = new JsonEncoder();
            var decoder = new JsonDecoder();
            var store = new Framework.EventStore(connection, encoder, decoder);

            await TestCase(store);
        }

        private static async Task TestCase(IEventStore store)
        {
            var id = Guid.NewGuid();

            await CreateCaseAsync(store, id);
            await ModifyCaseAsync(store, id);

            var @case = await store.CaseAsync(id);
            Console.WriteLine(JsonConvert.SerializeObject(@case, Formatting.Indented));
        }

        private static async Task CreateCaseAsync(IEventStore appender, Guid id)
        {
            var @case = Case.Create(id, "The Subject", "The Description");
            await @case.CommitAsync(appender, Case.Configuration);
        }

        private static async Task ModifyCaseAsync(IEventStore store, Guid id)
        {
            var @case = await store.CaseAsync(id);
            if (@case == null)
            {
                throw new InvalidOperationException();
            }

            @case.Import("Imported Subject", "Imported Description", "TS012345", CaseStatus.WaitingForDistributor);
            @case.AssignToService();

            await store.CommitAsync(@case);
        }
    }
}