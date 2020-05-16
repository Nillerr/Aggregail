using System;
using System.Collections.Generic;

namespace Aggregail
{
    public sealed class AggregateConfiguration<TIdentity, TAggregate>
        where TAggregate : Aggregate<TIdentity, TAggregate>
    {
        public delegate void EventApplicator(TAggregate aggregate, IJsonEventSerializer serializer, byte[] data);

        public delegate TAggregate Constructor(TIdentity id, IJsonEventSerializer serializer, byte[] data);

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
            where T : class
        {
            Constructors.Add(type.Value, (id, decoder, data) => constructor(id, decoder.Deserialize<T>(data)));
            return this;
        }

        public AggregateConfiguration<TIdentity, TAggregate> Applies<T>(
            EventType<T> type,
            Action<TAggregate, T> applicator
        )
            where T : class
        {
            Applicators.Add(type.Value,
                (aggregate, decoder, data) => applicator(aggregate, decoder.Deserialize<T>(data))
            );
            return this;
        }
    }
}