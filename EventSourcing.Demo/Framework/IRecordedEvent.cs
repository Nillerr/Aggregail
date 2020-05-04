using System;

namespace EventSourcing.Demo.Framework
{
    public interface IRecordedEvent
    {
        Guid EventId { get; }
        long EventNumber { get; }
        string EventType { get; }
        byte[] Data { get; }
    }
}