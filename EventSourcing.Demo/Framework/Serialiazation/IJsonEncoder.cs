namespace EventSourcing.Demo.Framework.Serialiazation
{
    public interface IJsonEncoder
    {
        byte[] Encode<T>(T source);
    }
}