namespace Aggregail
{
    public interface IEventSerializer
    {
        T Decode<T>(byte[] source);
        
        byte[] Encode<T>(T source);
    }
}