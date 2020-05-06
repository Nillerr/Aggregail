namespace EventSourcing.Demo.Framework
{
    public interface IValueObject<out T>
    {
        T Value { get; }
    }
}