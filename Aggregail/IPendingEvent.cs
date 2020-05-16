using System;

namespace Aggregail
{
    public interface IPendingEvent
    {
        Guid Id { get; }
        
        string Type { get; }
        
        byte[] Data(IJsonEventSerializer serializer);
    }
}