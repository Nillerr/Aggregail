using System;
using System.Collections.Generic;

namespace Aggregail
{
    public sealed class AggregateConfiguration<TIdentity, TAggregate>
        where TAggregate : Aggregate<TIdentity, TAggregate>
    {
        public delegate void EventApplicator(TAggregate aggregate, IEventSerializer serializer, byte[] data);
        public delegate TAggregate Constructor(TIdentity id, IEventSerializer serializer, byte[] data);

        public AggregateName<TIdentity, TAggregate> Name { get; }

        public Dictionary<string, Constructor> Constructors { get; } = new Dictionary<string, Constructor>();

        public Dictionary<string, EventApplicator> Applicators { get; } = new Dictionary<string, EventApplicator>();

        public AggregateConfiguration(AggregateName<TIdentity, TAggregate> name)
        {
            Name = name;
        }

        public AggregateConfiguration<TIdentity, TAggregate> Constructs<T>(
            EventType<T> type,
            Func<TIdentity, T, TAggregate> constructor
        )
        {
            Constructors.Add(type.Value, (id, decoder, data) => constructor(id, decoder.Decode<T>(data)));
            return this;
        }

        public AggregateConfiguration<TIdentity, TAggregate> Applies<T>(EventType<T> type, Action<TAggregate, T> applicator)
        {
            Applicators.Add(type.Value, (aggregate, decoder, data) => applicator(aggregate, decoder.Decode<T>(data)));
            return this;
        }
    }
}