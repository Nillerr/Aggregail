namespace EventSourcing.Demo.Framework
{
    public readonly struct AggregateName<TIdentity, TAggregate>
        where TAggregate : Aggregate<TIdentity, TAggregate>
    {
        public AggregateName(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static implicit operator AggregateName<TIdentity, TAggregate>(string value) =>
            new AggregateName<TIdentity, TAggregate>(value);

        public string Stream(TIdentity id) => $"{Value}-{id}";
    }
}