using System;

namespace EventSourcing.Demo.Framework
{
    public readonly struct AggregateName<TAggregate>
        where TAggregate : Aggregate<TAggregate>
    {
        public AggregateName(string value)
        {
            Value = value;
        }

        public string Value { get; }
        
        public static implicit operator AggregateName<TAggregate>(string value) => new AggregateName<TAggregate>(value);

        public string Stream(Guid id) => $"{Value}-{id:N}";
    }
}