using System;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace Aggregail.Testing
{
    public sealed class EventVerificationBuilder
    {
        private readonly List<(string eventType, Action<IPendingEvent, IJsonEventSerializer> verification)> _assertions
            = new List<(string, Action<IPendingEvent, IJsonEventSerializer>)>();

        public EventVerificationBuilder Event<T>(EventType<T> type, Action<T> assertion)
            where T : class
        {
            _assertions.Add((type.Value, (pendingEvent, serializer) =>
                {
                    var data = pendingEvent.Data(serializer);
                    var e = serializer.Deserialize<T>(data);
                    assertion(e);
                })
            );

            return this;
        }

        internal bool MatchingEvents(IEnumerable<IPendingEvent> pendingEvents, IJsonEventSerializer serializer)
        {
            foreach (var (pendingEvent, index) in pendingEvents.Select((e, i) => (e, i)))
            {
                var (eventType, assertion) = _assertions[index];
                if (eventType == pendingEvent.Type)
                {
                    assertion(pendingEvent, serializer);
                }
            }

            return true;
        }

        internal void Verify<TIdentity, TAggregate>(
            TIdentity id,
            Mock<IEventStore> target,
            long expectedVersion,
            IJsonEventSerializer serializer,
            Times? times = default
        )
            where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            target.Verify(e => e.AppendToStreamAsync(
                id,
                It.IsAny<AggregateConfiguration<TIdentity, TAggregate>>(),
                expectedVersion,
                It.Is<IEnumerable<IPendingEvent>>(e => MatchingEvents(e, serializer))
            ), times ?? Times.Once());
        }
    }

    public static class MockExtensions
    {
        public static void VerifyEvents(this Mock<IEventStore> eventStore, Func<>)
    }
}