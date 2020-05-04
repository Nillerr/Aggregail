using System;
using System.Threading.Tasks;
using EventSourcing.Demo.Cases;
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
            await TestCase(connection);
        }

        private static async Task TestCase(IEventStoreConnection connection)
        {
            var encoder = new JsonEncoder();
            var decoder = new JsonDecoder();

            var id = Guid.NewGuid();

            await CreateCaseAsync(connection, id, encoder);
            await ModifyCaseAsync(connection, id, encoder, decoder);

            var @case = await connection.CaseAsync(id, decoder);
            Console.WriteLine(JsonConvert.SerializeObject(@case, Formatting.Indented));
        }

        private static async Task CreateCaseAsync(IEventStoreConnection connection, Guid id, IJsonEncoder encoder)
        {
            var @case = Case.Create(id, "The Subject", "The Description");
            await @case.CommitAsync(connection, Case.Configuration, encoder);
        }

        private static async Task ModifyCaseAsync(
            IEventStoreConnection connection,
            Guid id,
            IJsonEncoder encoder,
            IJsonDecoder decoder
        )
        {
            var @case = await connection.CaseAsync(id, decoder);
            if (@case == null)
            {
                throw new InvalidOperationException();
            }

            @case.Import("Imported Subject", "Imported Description", "TS012345", CaseStatus.WaitingForDistributor);
            @case.AssignToService();

            await connection.CommitAsync(@case, encoder);
        }
    }
}