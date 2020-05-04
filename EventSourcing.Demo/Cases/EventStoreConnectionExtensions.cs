using System;
using System.Threading.Tasks;
using EventSourcing.Demo.Framework;

namespace EventSourcing.Demo.Cases
{
    public static class EventStoreConnectionExtensions
    {
        public static async Task<Case?> CaseAsync(this IEventStoreReader reader, Guid id)
        {
            return await reader.AggregateAsync(id, Case.Configuration);
        }

        public static async Task CommitAsync(this IEventStoreAppender connection, Case @case)
        {
            await @case.CommitAsync(connection, Case.Configuration);
        }
    }
}