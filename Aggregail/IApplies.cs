namespace Aggregail
{
    public interface IApplies<in TEvent>
    {
        void Apply(TEvent @event);
    }
}