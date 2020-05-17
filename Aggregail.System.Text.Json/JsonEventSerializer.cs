using System.Text.Json;

namespace Aggregail.System.Text.Json
{
    /// <summary>
    /// Serializes events to / from JSON using Newtonsoft.Json
    /// </summary>
    public sealed class JsonEventSerializer : IJsonEventSerializer
    {
        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonEventSerializer"/> class, using a
        /// <see name="JsonSerializer"/> to control serialization.
        /// </summary>
        /// <param name="serializerOptions">The <see cref="JsonSerializerOptions"/> that controls serialization</param>
        public JsonEventSerializer(JsonSerializerOptions serializerOptions)
        {
            _serializerOptions = serializerOptions;
        }

        /// <inheritdoc />
        public T Deserialize<T>(byte[] source)
            where T : class
        {
            return JsonSerializer.Deserialize<T>(source);
        }

        /// <inheritdoc />
        public byte[] Serialize<T>(T source)
            where T : class
        {
            return JsonSerializer.SerializeToUtf8Bytes(source, _serializerOptions);
        }
    }
}