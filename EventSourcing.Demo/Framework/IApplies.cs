namespace EventSourcing.Demo.Framework
{
    public interface IApplies<in TEvent>
    {
        void Apply(TEvent e);
    }
}