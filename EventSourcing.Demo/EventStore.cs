using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Aggregail;
using EventStore.ClientAPI;

namespace EventSourcing.Demo
{
    public sealed class EventStore : IEventStore
    {
        private readonly IEventStoreConnection _connection;
        private readonly IJsonEventSerializer _serializer;
        private readonly IStreamNameResolver _streamNameResolver;

        public EventStore(
            IEventStoreConnection connection,
            IJsonEventSerializer serializer,
            IStreamNameResolver streamNameResolver
        )
        {
            _connection = connection;
            _serializer = serializer;
            _streamNameResolver = streamNameResolver;
        }

        public async Task AppendToStreamAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long expectedVersion,
            IEnumerable<IPendingEvent> pendingEvents,
            CancellationToken cancellationToken = default
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            var events = pendingEvents
                .Select(EventData)
                .ToArray();

            var stream = _streamNameResolver.Stream(id, configuration);
            await _connection.AppendToStreamAsync(stream, expectedVersion, events);
        }

        private EventData EventData(IPendingEvent pendingEvent)
        {
            var data = pendingEvent.Data(_serializer);
            return new EventData(pendingEvent.Id, pendingEvent.Type, true, data, null);
        }

        public async Task<TAggregate?> AggregateAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long? version = null,
            CancellationToken cancellationToken = default
        )
            where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            var stream = _streamNameResolver.Stream(id, configuration);

            var createdResult = await _connection.ReadEventAsync(stream, StreamPosition.Start, false);
            if (createdResult.Event == null)
            {
                return null;
            }

            var createdResolvedEvent = createdResult.Event.Value;
            var createdRecordedEvent = createdResolvedEvent.Event;

            if (!configuration.Constructors.TryGetValue(createdRecordedEvent.EventType, out var constructor))
            {
                throw new InvalidOperationException(
                    $"Unrecognized construction event type: {createdRecordedEvent.EventType}"
                );
            }

            var aggregate = constructor(id, _serializer, createdRecordedEvent.Data);
            aggregate.Record(new RecordableEvent(createdRecordedEvent.EventNumber));

            var applicators = configuration.Applicators;

            long sliceStart = 1;
            const int sliceSize = 100;

            StreamEventsSlice slice;
            do
            {
                slice = await _connection.ReadStreamEventsForwardAsync(stream, sliceStart, sliceSize, false);

                foreach (var resolvedEvent in slice.Events)
                {
                    var recordedEvent = resolvedEvent.Event;
                    if (version.HasValue && recordedEvent.EventNumber > version.Value)
                    {
                        return aggregate;
                    }
                    
                    if (applicators.TryGetValue(recordedEvent.EventType, out var applicator))
                    {
                        applicator(aggregate, _serializer, recordedEvent.Data);
                        aggregate.Record(new RecordableEvent(recordedEvent.EventNumber));
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Unexpected recorded event type: {recordedEvent.EventType}"
                        );
                    }
                }

                sliceStart = slice.NextEventNumber;
            } while (!slice.IsEndOfStream);

            return aggregate;
        }

        public IAsyncEnumerable<TIdentity> AggregateIdsAsync<TIdentity, TAggregate>(
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            [EnumeratorCancellation] CancellationToken cancellationToken = default
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAggregateAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long expectedVersion,
            CancellationToken cancellationToken = default
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            var stream = _streamNameResolver.Stream(id, configuration);
            await _connection.DeleteStreamAsync(stream, expectedVersion);
        }
    }
}