using System;
using EventSourcing.Demo.Framework.Serialiazation;

namespace EventSourcing.Demo.Framework
{
    public interface IPendingEvent
    {
        Guid Id { get; }
        
        string Type { get; }
        
        byte[] EncodedData(IJsonEncoder encoder);
    }
}