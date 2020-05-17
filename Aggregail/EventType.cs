namespace Aggregail
{
    /// <summary>
    /// Strongly typed string wrapper for specifying an identifier for a type of event.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// It is intended to be used with the implicit constructor <see cref="op_Implicit"/>, like so:
    /// <code>
    /// public sealed class GoatCreated
    /// {
    ///     // It is not recommended to use `nameof(GoatCreated)`, because the identifier
    ///     // must remain the same even if the class name changes.
    ///     public static readonly EventType&lt;GoatCreated&gt; EventType = "GoatCreated";
    /// }
    /// </code>
    /// </remarks>
    public readonly struct EventType<T>
    {
        /// <summary>
        /// Creates an instance of the <see cref="EventType{T}"/> struct.
        /// </summary>
        /// <param name="value">The type identifier of the event type.</param>
        public EventType(string value)
        {
            Value = value;
        }

        /// <summary>
        /// The type identifier of the event type.
        /// </summary>
        public string Value { get; }
        
        /// <summary>
        /// Creates an instance of the <see cref="EventType{T}"/> struct from a string.
        /// </summary>
        /// <param name="value">The type identifier of the event type.</param>
        /// <returns>An <see cref="EventType{T}"/> containing the string value.</returns>
        public static implicit operator EventType<T>(string value) => new EventType<T>(value);
    }
}