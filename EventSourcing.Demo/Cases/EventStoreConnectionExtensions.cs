using System;
using System.Threading.Tasks;
using EventSourcing.Demo.Framework;
using EventSourcing.Demo.Framework.Serialiazation;
using EventStore.ClientAPI;

namespace EventSourcing.Demo.Cases
{
    public static class EventStoreConnectionExtensions
    {
        public static async Task<Case?> CaseAsync(this IEventStoreConnection connection, Guid id, IJsonDecoder decoder)
        {
            return await connection.AggregateAsync(id, Case.Configuration, decoder);
        }

        public static async Task CommitAsync(this IEventStoreConnection connection, Case @case, IJsonEncoder encoder)
        {
            await @case.CommitAsync(connection, Case.Configuration, encoder);
        }
    }
}