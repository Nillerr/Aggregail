using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Aggregail.Testing
{
    /// <summary>
    /// Builds a list of event assertions during a call to
    /// <see cref="VerifiableEventStore.VerifyAppendToStream{TIdentity}"/>.
    /// </summary>
    public sealed class EventAssertionsBuilder
    {
        /// <summary>
        /// An block of assertions on the event <paramref name="e"/>.
        /// </summary>
        /// <param name="e">The event to assert</param>
        /// <typeparam name="TEvent">The type of event</typeparam>
        public delegate void Assertion<in TEvent>(TEvent e);
        
        internal sealed class EventAssertion
        {
            public EventAssertion(string eventType, Action<IPendingEvent, IJsonEventSerializer> assertions)
            {
                EventType = eventType;
                Assertions = assertions;
            }

            public string EventType { get; }
            public Action<IPendingEvent, IJsonEventSerializer> Assertions { get; }
        }
        
        internal readonly List<EventAssertion> Assertions = new List<EventAssertion>();

        /// <summary>
        /// Asserts the expectation of an event of the type specified by <paramref name="type"/> was appended.
        /// </summary>
        /// <param name="type">Type of event to expect.</param>
        /// <param name="assertion">Assertion of the event content.</param>
        /// <typeparam name="TEvent">The type of event.</typeparam>
        /// <returns>The assertion builder.</returns>
        /// <remarks>
        /// The order of events is verified as well, so the order of the event assertion must match that of a call to
        /// <see cref="IEventStore.AppendToStreamAsync{TIdentity}"/>.
        /// </remarks>
        public EventAssertionsBuilder Event<TEvent>(EventType<TEvent> type, Assertion<TEvent> assertion = null)
            where TEvent : class
        {
            Assertions.Add(new EventAssertion(type.Value, (pendingEvent, serializer) =>
                {
                    var data = pendingEvent.Data(serializer);
                    var e = serializer.Deserialize<TEvent>(data);
                    assertion?.Invoke(e);
                })
            );

            return this;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return JsonConvert.SerializeObject(Assertions.Select(ass => ass.EventType), Formatting.None);
        }
    }
}