namespace EventSourcing.Demo.Framework
{
    public interface IApply<in TEvent>
    {
        void Apply(TEvent @event);
    }
}