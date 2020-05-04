namespace EventSourcing.Demo.Framework.Serialiazation
{
    public interface IJsonDecoder
    {
        T Decode<T>(byte[] source);
    }
}