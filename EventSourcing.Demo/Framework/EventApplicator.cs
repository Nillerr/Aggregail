using EventSourcing.Demo.Framework.Serialiazation;

namespace EventSourcing.Demo.Framework
{
    public delegate void EventApplicator<in TAggregate>(TAggregate aggregate, IJsonDecoder decoder, byte[] data);
}