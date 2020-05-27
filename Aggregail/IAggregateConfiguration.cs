using System;

namespace Aggregail
{
    /// <summary>
    /// Configuration of an aggregate, containing information on how to construct instance of the aggregate from
    /// recorded events, and how to apply recorded events to an aggregate instance. 
    /// </summary>
    public interface IAggregateConfiguration
    {
        /// <summary>
        /// The type of aggregate created by this configuration.
        /// </summary>
        Type AggregateType { get; }
        
        /// <summary>
        /// The name of the aggregate type.
        /// </summary>
        string Name { get; }
    }

    /// <inheritdoc />
    public interface IAggregateConfiguration<out TIdentity> : IAggregateConfiguration
    {
        /// <summary>
        /// Parses a string into an instance of <c>TIdentity</c>.
        /// </summary>
        Parser<TIdentity> IdentityParser { get; }
    }
}