using System;

namespace Aggregail
{
    public interface IRecordedEvent
    {
        string Stream { get; }
        
        Guid EventId { get; }
        long EventNumber { get; }
        string EventType { get; }
        
        DateTime Created { get; }

        T Data<T>() where T : class;
        T Metadata<T>() where T : class;
    }
    
    public interface IRecordedEvent<out TIdentity, TAggregate> : IRecordedEvent
        where TAggregate : Aggregate<TIdentity, TAggregate>
    {
        TIdentity Id { get; }
    }
}