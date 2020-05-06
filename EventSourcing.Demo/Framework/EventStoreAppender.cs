using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventSourcing.Demo.Framework.Serialiazation;
using EventStore.ClientAPI;

namespace EventSourcing.Demo.Framework
{
    public sealed class EventStoreAppender : IEventStoreAppender
    {
        private readonly IEventStoreConnection _connection;
        private readonly IJsonEncoder _encoder;

        public EventStoreAppender(IEventStoreConnection connection, IJsonEncoder encoder)
        {
            _connection = connection;
            _encoder = encoder;
        }

        public async Task AppendToStreamAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long expectedVersion,
            IEnumerable<IPendingEvent> pendingEvents
        )
            where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            var events = pendingEvents.Select(pendingEvent =>
                new EventData(pendingEvent.Id, pendingEvent.Type, true, pendingEvent.EncodedData(_encoder), null)
            );

            var stream = configuration.Name.Stream(id);
            await _connection.AppendToStreamAsync(stream, expectedVersion, events);
        }
    }
}