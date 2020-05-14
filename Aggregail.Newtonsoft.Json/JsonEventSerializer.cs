using System.Text;
using Newtonsoft.Json;

namespace Aggregail.Newtonsoft.Json
{
    public sealed class JsonEventSerializer : IEventSerializer
    {
        public T Decode<T>(byte[] source)
        {
            var json = Encoding.UTF8.GetString(source);
            return JsonConvert.DeserializeObject<T>(json);
        }
        
        public byte[] Encode<T>(T source)
        {
            var json = JsonConvert.SerializeObject(source);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}