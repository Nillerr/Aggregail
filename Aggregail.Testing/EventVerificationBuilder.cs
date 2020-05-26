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

        private bool MatchingEvents(IEnumerable<IPendingEvent> pendingEvents, IJsonEventSerializer serializer)
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

        internal void Verify<TIdentity>(
            TIdentity id,
            Mock<IEventStore> target,
            long expectedVersion,
            IJsonEventSerializer serializer,
            Times? times = default
        )
        {
            target.Verify(es => es.AppendToStreamAsync(
                id,
                It.IsAny<IAggregateConfiguration<TIdentity>>(),
                expectedVersion,
                It.Is<IEnumerable<IPendingEvent>>(pendingEvents => MatchingEvents(pendingEvents, serializer))
            ), times ?? Times.Once());
        }
    }
}