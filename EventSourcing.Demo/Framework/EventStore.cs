using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventSourcing.Demo.Framework.Serialiazation;
using EventStore.ClientAPI;

namespace EventSourcing.Demo.Framework
{
    public sealed class EventStore : IEventStore
    {
        private readonly EventStoreReader _reader;
        private readonly EventStoreAppender _appender;

        public EventStore(IEventStoreConnection connection, IJsonEncoder encoder, IJsonDecoder decoder)
        {
            _reader = new EventStoreReader(connection, decoder);
            _appender = new EventStoreAppender(connection, encoder);
        }

        public Task AppendToStreamAsync<TAggregate, TCreateEvent>(
            Guid id,
            AggregateConfiguration<TAggregate, TCreateEvent> configuration,
            long expectedVersion,
            IEnumerable<IPendingEvent> pendingEvents
        ) where TAggregate : Aggregate<TAggregate, TCreateEvent>
        {
            return _appender.AppendToStreamAsync(id, configuration, expectedVersion, pendingEvents);
        }

        public Task<TAggregate?> AggregateAsync<TAggregate, TCreatedEvent>(
            Guid id,
            AggregateConfiguration<TAggregate, TCreatedEvent> configuration
        ) where TAggregate : Aggregate<TAggregate, TCreatedEvent>
        {
            return _reader.AggregateAsync(id, configuration);
        }
    }
}