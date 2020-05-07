using System.Threading.Tasks;
using EventSourcing.Demo.Framework.Serialiazation;
using EventStore.ClientAPI;

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

            await Cases.Demo.RunAsync(store);
        }
    }
}