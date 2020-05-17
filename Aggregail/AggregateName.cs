namespace Aggregail
{
    /// <summary>
    /// Strongly typed string wrapper for specifying an identifier for a type of Aggregate.
    /// </summary>
    /// <typeparam name="TIdentity">Type of ID of the Aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of aggregate.</typeparam>
    /// <remarks>
    /// It is intended to be used with the implicit constructor <see cref="op_Implicit"/> when passed as the argument
    /// to the constructor of <see cref="AggregateConfiguration{TIdentity,TAggregate}"/>, like so:
    /// <code>
    /// public sealed class Goat : Aggregate&lt;GoatId, Goat&gt;
    /// {
    ///     // It is not recommended to use `nameof(Goat)`, because the name
    ///     // must remain the same even if the class name changes.
    ///     private static readonly AggregateConfiguration&lt;GoatId, Goat&gt; Configuration =
    ///         new AggregateConfiguration&lt;GoatId, Goat&gt;("goat");
    /// }
    /// </code>
    /// </remarks>
    public readonly struct AggregateName<TIdentity, TAggregate>
        where TAggregate : Aggregate<TIdentity, TAggregate>
    {
        /// <summary>
        /// Creates an instance of the <see cref="AggregateName{TIdentity,TAggregate}"/> struct.
        /// </summary>
        /// <param name="value">The name of the aggregate type.</param>
        public AggregateName(string value)
        {
            Value = value;
        }

        /// <summary>
        /// The name of the aggregate type.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates an instance of the <see cref="AggregateName{TIdentity,TAggregate}"/> struct from a string.
        /// </summary>
        /// <param name="value">The name of the aggregate type.</param>
        /// <returns>An <see cref="AggregateName{TIdentity,TAggregate}"/> containing the string value.</returns>
        public static implicit operator AggregateName<TIdentity, TAggregate>(string value) =>
            new AggregateName<TIdentity, TAggregate>(value);

        /// <summary>
        /// Returns the name of an aggregate stream.
        /// </summary>
        /// <param name="id">Id of the aggregate</param>
        /// <returns>The name of the stream for the given <paramref name="id"/></returns>
        public string Stream(TIdentity id) => $"{Value}-{id}";
    }
}