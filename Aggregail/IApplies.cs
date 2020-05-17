namespace Aggregail
{
    /// <summary>
    /// Convenience interface for implementing the <c>Apply</c> method pattern using a strict type, and allows
    /// configuration using the <see cref="AggregateConfigurationExtensions.Applies{T, TIdentity, TAggregate}"/>
    /// extension method. 
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <remarks>
    /// Whether you prefer implementing this interface, or using lamda expressions is a matter of preference. Using
    /// lamda expressions have the advantage of being able to keep the <c>Apply</c> methods private, and thus not
    /// exposing them as part of the API surface of the Aggregate Root.
    /// </remarks>
    public interface IApplies<in TEvent>
    {
        /// <summary>
        /// Applies an event of type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <param name="e">The event to apply</param>
        void Apply(TEvent e);
    }
}