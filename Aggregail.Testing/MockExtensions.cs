using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Aggregail.Newtonsoft.Json;
using Moq;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Aggregail.Testing
{
    public static class MockExtensions
    {
        public static void VerifyAppendToStream<TIdentity>(
            this Mock<IEventStore> eventStore,
            TIdentity id,
            long expectedVersion,
            Action<EventVerificationBuilder> verification,
            IJsonEventSerializer serializer = null
        )
        {
            var evb = new EventVerificationBuilder();
            verification(evb);
            
            var actualSerializer = serializer ?? new JsonEventSerializer(JsonSerializer.Create());
            evb.Verify(id, eventStore, expectedVersion, actualSerializer);
        }

        public static long SetupAggregate<TIdentity, TAggregate>(
            this Mock<IEventStore> eventStore,
            Aggregate<TIdentity, TAggregate> aggregate
        )
            where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            // We'll clear the list of verifications later, so we must start out with none.
            eventStore.VerifyNoOtherCalls();

            // "Commit" the aggregate events
            eventStore
                .Setup(e => e.AppendToStreamAsync(
                        aggregate.Id,
                        It.IsAny<IAggregateConfiguration<TIdentity>>(),
                        ExpectedVersion.NoStream,
                        It.IsAny<IEnumerable<IPendingEvent>>()
                    )
                )
                .Returns(Task.CompletedTask);

            aggregate.CommitAsync(eventStore.Object, It.IsAny<AggregateConfiguration<TIdentity, TAggregate>>());

            // Verify that it was actually committed to the store
            eventStore.Verify(e => e.AppendToStreamAsync(
                aggregate.Id,
                It.IsAny<IAggregateConfiguration<TIdentity>>(),
                ExpectedVersion.NoStream,
                It.IsAny<IEnumerable<IPendingEvent>>()
            ), Times.Once());
            
            eventStore.VerifyNoOtherCalls();
            
            // Extracts the invocation above to get hold of the number of events "committed".
            var invocation = eventStore.Invocations.Single();
            
            var pendingEvents = (IEnumerable<IPendingEvent>) invocation.Arguments[3];
            var numberOfPendingEvents = pendingEvents.LongCount();
            
            // Clear the invocation list, so consumers can do their own verifications from a clean slate.
            eventStore.Invocations.Clear();
            
            // Once the aggregate is "committed", we can mock retrieval of it.
            eventStore
                .Setup(e => e.AggregateAsync(aggregate.Id, It.IsAny<AggregateConfiguration<TIdentity, TAggregate>>()))
                .ReturnsAsync((TAggregate) aggregate);

            return ExpectedVersion.NoStream + numberOfPendingEvents;
        }
    }
}