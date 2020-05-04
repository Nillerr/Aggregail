using System.Text;
using Newtonsoft.Json;

namespace EventSourcing.Demo.Framework.Serialiazation
{
    public sealed class JsonEncoder : IJsonEncoder
    {
        public byte[] Encode<T>(T source)
        {
            var json = JsonConvert.SerializeObject(source);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}