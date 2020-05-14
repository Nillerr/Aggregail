using System;
using Newtonsoft.Json;

namespace Aggregail.Newtonsoft.Json
{
    public abstract class ValueTypeJsonConverter<T> : JsonConverter
        where T : struct
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T) || Nullable.GetUnderlyingType(objectType) == typeof(T);
        }

        public override bool CanWrite => true;

        public override bool CanRead => true;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            switch (value)
            {
                case null:
                    writer.WriteNull();
                    break;
                case T nonNullValue:
                    WriteJson(writer, nonNullValue, serializer);
                    break;
                default:
                    throw new JsonSerializationException($"Cannot convert {value} to {typeof(T)}");
            }
        }

        public abstract void WriteJson(JsonWriter writer, T value, JsonSerializer serializer);

        public override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer
        )
        {
            var underlyingNullableType = Nullable.GetUnderlyingType(objectType);
            if (reader.TokenType == JsonToken.Null)
            {
                if (!(underlyingNullableType is null))
                {
                    return null;
                }

                throw new JsonSerializationException($"Cannot convert null value to {objectType}.");
            }

            return ReadJson(reader, objectType, (T?) existingValue, serializer);
        }

        public abstract T ReadJson(JsonReader reader, Type objectType, T? existingValue, JsonSerializer serializer);
    }
}