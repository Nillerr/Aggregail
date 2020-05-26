using System;
using System.Collections.Generic;

namespace Aggregail
{
    /// <summary>
    /// Configuration of an aggregate, containing information on how to construct instance of the aggregate from
    /// recorded events, and how to apply recorded events to an aggregate instance. 
    /// </summary>
    /// <typeparam name="TIdentity">Type of ID of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of aggregate.</typeparam>
    public sealed class AggregateConfiguration<TIdentity, TAggregate> : IAggregateConfiguration<TIdentity>
        where TAggregate : Aggregate<TIdentity, TAggregate>
    {
        /// <summary>
        /// Creates an instance of <c>TAggregate</c> by deserializing event data, and passing it to the
        /// called constructor.
        /// </summary>
        /// <param name="id">Id of the aggregate being constructed.</param>
        /// <param name="serializer">Serializer configured in the <see cref="IEventStore"/>.</param>
        /// <param name="data">Data of the construction event.</param>
        public delegate TAggregate Constructor(TIdentity id, IJsonEventSerializer serializer, byte[] data);

        /// <summary>
        /// Applies an event to an <c>TAggregate</c> by deserializing event data, and passing it to the
        /// called <c>Apply(TEvent e)</c> method.
        /// </summary>
        /// <param name="aggregate">Aggregate instance to apply the event to.</param>
        /// <param name="serializer">Serialized configured in the <see cref="IEventStore"/>.</param>
        /// <param name="data">Data of the event.</param>
        public delegate void EventApplicator(TAggregate aggregate, IJsonEventSerializer serializer, byte[] data);

        /// <summary>
        /// Type-safe name of the aggregate type.
        /// </summary>
        public AggregateName<TIdentity, TAggregate> Name { get; }
        
        /// <summary>
        /// Parses a string into an instance of <c>TIdentity</c>.
        /// </summary>
        public Parser<TIdentity> IdentityParser { get; }

        /// <summary>
        /// Configured constructors for the <c>TAggregate</c>, keyed by the string value of the
        /// <see cref="EventType{T}"/> for the event associated with the constructor.
        /// </summary>
        public Dictionary<string, Constructor> Constructors { get; } = new Dictionary<string, Constructor>();

        /// <summary>
        /// Configured event applicators for the <c>TAggregate</c>, keyed by the string value of the
        /// <see cref="EventType{T}"/> for the event associated with the applicator.
        /// </summary>
        public Dictionary<string, EventApplicator> Applicators { get; } = new Dictionary<string, EventApplicator>();

        /// <summary>
        /// Creates an instance of the <see cref="AggregateConfiguration{TIdentity,TAggregate}"/> class.
        /// </summary>
        /// <param name="name">Name of the aggregate type.</param>
        /// <param name="identityParser">The identity parser.</param>
        public AggregateConfiguration(AggregateName<TIdentity, TAggregate> name, Parser<TIdentity> identityParser)
        {
            Name = name;
            IdentityParser = identityParser;
        }

        /// <summary>
        /// Configures a constructor for the event of type <typeparamref name="T"/>, identified by the
        /// <paramref name="type"/> argument, calling the <paramref name="constructor"/> function with the deserialized
        /// event data of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="type">Type of the event.</param>
        /// <param name="constructor">Function to construct aggregate instance.</param>
        /// <typeparam name="T">Type of event.</typeparam>
        /// <returns>The modified aggregate configuration.</returns>
        public AggregateConfiguration<TIdentity, TAggregate> Constructs<T>(
            EventType<T> type,
            Func<TIdentity, T, TAggregate> constructor
        )
            where T : class
        {
            Constructors.Add(type.Value, (id, decoder, data) => constructor(id, decoder.Deserialize<T>(data)));
            return this;
        }

        /// <summary>
        /// Configures an applicator for the event of type <typeparamref name="T"/>, identified by the
        /// <paramref name="type"/> argument, calling the <paramref name="applicator"/> function with the deserialized
        /// event data of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="type">Type of the event.</param>
        /// <param name="applicator">Function to apply the event data to an aggregate with.</param>
        /// <typeparam name="T">Type of event.</typeparam>
        /// <returns>The modified aggregate configuration.</returns>
        public AggregateConfiguration<TIdentity, TAggregate> Applies<T>(
            EventType<T> type,
            Action<TAggregate, T> applicator
        )
            where T : class
        {
            Applicators.Add(type.Value, (aggregate, decoder, data) => applicator(aggregate, decoder.Deserialize<T>(data)));
            return this;
        }

        /// <summary>
        /// Configures an applicator for the event of type <typeparamref name="T"/>, identified by the
        /// <paramref name="type"/> argument, which does nothing when the event is received.
        /// </summary>
        /// <param name="type">Type of the event.</param>
        /// <typeparam name="T">Type of event.</typeparam>
        /// <returns>The modified aggregate configuration.</returns>
        public AggregateConfiguration<TIdentity, TAggregate> Ignores<T>(EventType<T> type) where T : class
        {
            Applicators.Add(type.Value, (a, d, c) => { });
            return this;
        }

        /// <inheritdoc />
        public Type AggregateType { get; } = typeof(TAggregate);

        /// <inheritdoc />
        public string Stream(TIdentity id) => Name.Stream(id);
    }
}