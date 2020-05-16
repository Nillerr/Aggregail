namespace Aggregail
{
    /// <summary>
    /// A single value object
    /// </summary>
    /// <typeparam name="T">The type of value wrapped by the value object</typeparam>
    /// <remarks>
    /// Used to improve type-safety when otherwise resorting to using stringly-typed values.
    /// </remarks>
    public interface IValueObject<out T>
    {
        /// <summary>
        /// The value wrapped by this value object
        /// </summary>
        T Value { get; }
    }
}