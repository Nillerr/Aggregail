using System;

namespace Aggregail
{
    /// <summary>
    /// Configuration of an aggregate, containing information on how to construct instance of the aggregate from
    /// recorded events, and how to apply recorded events to an aggregate instance. 
    /// </summary>
    /// <typeparam name="TIdentity">Type of ID of the aggregate.</typeparam>
    public interface IAggregateConfiguration<in TIdentity>
    {
        /// <summary>
        /// The type of aggregate created by this configuration.
        /// </summary>
        Type AggregateType { get; }
        
        /// <summary>
        /// Returns the name of a stream for the given <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Id of the aggregate</param>
        /// <returns>The name of the stream for the given id.</returns>
        string Stream(TIdentity id);
    }
}