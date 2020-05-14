namespace Aggregail
{
    public interface IValueObject<out T>
    {
        T Value { get; }
    }
}