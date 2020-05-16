using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Aggregail.Newtonsoft.Json
{
    /// <summary>
    /// Serializes events to / from JSON using Newtonsoft.Json
    /// </summary>
    public sealed class JsonEventSerializer : IJsonEventSerializer
    {
        private readonly JsonSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonEventSerializer"/> class, using a
        /// <see name="JsonSerializer"/> to control serialization.
        /// </summary>
        /// <param name="serializer">The <see cref="JsonSerializer"/> that controls serialization</param>
        public JsonEventSerializer(JsonSerializer serializer)
        {
            _serializer = serializer;
        }

        /// <inheritdoc/>
        public T Deserialize<T>(byte[] source) where T : class
        {
            using var stream = new MemoryStream(source);
            using var textReader = new StreamReader(stream, Encoding.UTF8);
            using var jsonReader = new JsonTextReader(textReader);

            var e = _serializer.Deserialize<T>(jsonReader);
            if (e == null)
            {
                throw new ArgumentException(
                    $"Serialization does not support deserializing a single `null` token into {typeof(T).Name}.",
                    nameof(source)
                );
            }

            return e;
        }

        /// <inheritdoc/>
        public byte[] Serialize<T>(T source) where T : class
        {
            using var stream = new MemoryStream();
            using var textWriter = new StreamWriter(stream, Encoding.UTF8);
            using var jsonWriter = new JsonTextWriter(textWriter);

            _serializer.Serialize(jsonWriter, source);

            return stream.ToArray();
        }
    }
}