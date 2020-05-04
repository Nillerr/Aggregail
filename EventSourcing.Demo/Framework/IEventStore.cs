namespace EventSourcing.Demo.Framework
{
    public interface IEventStore : IEventStoreAppender, IEventStoreReader
    {
    }
}