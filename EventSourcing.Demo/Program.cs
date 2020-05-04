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
            await TestCase(connection);
        }

        private static async Task TestCase(IEventStoreConnection connection)
        {
            var decoder = new JsonDecoder();
            var reader = new EventStoreReader(connection, decoder);
            
            var encoder = new JsonEncoder();
            var appender = new EventStoreAppender(connection, encoder);

            var id = Guid.NewGuid();

            await CreateCaseAsync(appender, id);
            await ModifyCaseAsync(reader, appender, id);

            var @case = await reader.CaseAsync(id);
            Console.WriteLine(JsonConvert.SerializeObject(@case, Formatting.Indented));
        }

        private static async Task CreateCaseAsync(IEventStoreAppender appender, Guid id)
        {
            var @case = Case.Create(id, "The Subject", "The Description");
            await @case.CommitAsync(appender, Case.Configuration);
        }

        private static async Task ModifyCaseAsync(
            IEventStoreReader reader,
            IEventStoreAppender appender,
            Guid id
        )
        {
            var @case = await reader.CaseAsync(id);
            if (@case == null)
            {
                throw new InvalidOperationException();
            }

            @case.Import("Imported Subject", "Imported Description", "TS012345", CaseStatus.WaitingForDistributor);
            @case.AssignToService();

            await appender.CommitAsync(@case);
        }
    }
}