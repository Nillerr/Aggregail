using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moq;

namespace Aggregail.Testing
{
    internal static class Method
    {
        public static MethodInfo OfCallTo<T>(Expression<Func<T, object>> expression)
        {
            var memberExpression = (MethodCallExpression) expression.Body;
            return memberExpression.Method;
        }
    }
    
    public sealed class EventVerificationBuilder
    {
        private readonly List<(string eventType, Action<IPendingEvent, IJsonEventSerializer> verification)> _assertions
            = new List<(string, Action<IPendingEvent, IJsonEventSerializer>)>();

        public EventVerificationBuilder Event<T>(EventType<T> type, Action<T> assertion = null)
            where T : class
        {
            _assertions.Add((type.Value, (pendingEvent, serializer) =>
                {
                    var data = pendingEvent.Data(serializer);
                    var e = serializer.Deserialize<T>(data);
                    assertion?.Invoke(e);
                })
            );

            return this;
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
                It.Is<IEnumerable<IPendingEvent>>(pendingEvents => MatchingEvents(pendingEvents, expectedVersion, serializer))
            ), times ?? Times.Once());
        }

        private bool MatchingEvents(IEnumerable<IPendingEvent> pendingEvents, long expectedVersion, IJsonEventSerializer serializer)
        {
            AssertMatchingPendingEvents(pendingEvents, expectedVersion, serializer);
            return true;
        }

        internal void VerifyCustom<TIdentity>(
            TIdentity id,
            Mock<IEventStore> target,
            long expectedVersion,
            IJsonEventSerializer serializer
        )
        {
            var invocation = target.Invocations
                .Single(inv =>
                    inv.Method == Method.OfCallTo<IEventStore>(es =>
                        es.AppendToStreamAsync(id, default, default, default)
                    ) &&

                    inv.Arguments[0].Equals(id) &&
                    inv.Arguments[2].Equals(expectedVersion)
                );

            var pendingEvents = (IEnumerable<IPendingEvent>) invocation.Arguments[3];
            AssertMatchingPendingEvents(pendingEvents, expectedVersion, serializer);
        }

        private void AssertMatchingPendingEvents(
            IEnumerable<IPendingEvent> pendingEvents,
            long expectedVersion,
            IJsonEventSerializer serializer
        )
        {
            var pendingEventList = pendingEvents.ToList();

            for (var i = 0; i < pendingEventList.Count; i++)
            {
                var pendingEvent = pendingEventList[i];
                var (eventType, assertion) = _assertions[i];
                if (eventType == pendingEvent.Type)
                {
                    assertion(pendingEvent, serializer);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Expected event {i} (#{expectedVersion + i + 1}) to be an event of type " +
                        $"`{eventType}`, but was `{pendingEvent.Type}`."
                    );
                }
            }
            
            if (pendingEventList.Count != _assertions.Count)
            {
                throw new InvalidOperationException(
                    $"Expected {_assertions.Count} events, but {pendingEventList.Count} events were appended."
                );
            }
        }
    }
}