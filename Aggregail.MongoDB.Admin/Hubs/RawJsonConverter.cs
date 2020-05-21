using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Aggregail.MongoDB.Admin.Hubs
{
    public sealed class RawJsonConverter : JsonConverter<byte[]?>
    {
        public override byte[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var document = JsonDocument.ParseValue(ref reader);
            if (document.RootElement.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            
            var json = document.RootElement.ToString();
            return Encoding.UTF8.GetBytes(json);
        }

        public override void Write(Utf8JsonWriter writer, byte[]? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }

            var document = JsonDocument.Parse(value);
            document.WriteTo(writer);
        }
    }
}