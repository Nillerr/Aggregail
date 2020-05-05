using System;
using System.Collections.Generic;
using EventSourcing.Demo.Framework.Serialiazation;

namespace EventSourcing.Demo.Framework
{
    public sealed class AggregateConfiguration<TAggregate>
        where TAggregate : Aggregate<TAggregate>
    {
        public delegate void EventApplicator(TAggregate aggregate, IJsonDecoder decoder, byte[] data);

        public AggregateName<TAggregate> Name { get; }

        public Dictionary<string, Func<Guid, IJsonDecoder, byte[], TAggregate>> Constructors { get; } =
            new Dictionary<string, Func<Guid, IJsonDecoder, byte[], TAggregate>>();

        public Dictionary<string, EventApplicator> Applicators { get; } = new Dictionary<string, EventApplicator>();

        public AggregateConfiguration(AggregateName<TAggregate> name)
        {
            Name = name;
        }

        public AggregateConfiguration<TAggregate> Constructs<T>(
            EventType<T> type,
            Func<Guid, T, TAggregate> constructor
        )
        {
            Constructors.Add(type.Value, (id, decoder, data) => constructor(id, decoder.Decode<T>(data)));
            return this;
        }

        public AggregateConfiguration<TAggregate> Applies<T>(EventType<T> type, Action<TAggregate, T> applicator)
        {
            Applicators.Add(type.Value, (aggregate, decoder, data) => applicator(aggregate, decoder.Decode<T>(data)));
            return this;
        }
    }
}