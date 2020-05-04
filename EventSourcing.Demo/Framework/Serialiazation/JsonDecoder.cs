using System.Text;
using Newtonsoft.Json;

namespace EventSourcing.Demo.Framework.Serialiazation
{
    public sealed class JsonDecoder : IJsonDecoder
    {
        public T Decode<T>(byte[] source)
        {
            var json = Encoding.UTF8.GetString(source);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}