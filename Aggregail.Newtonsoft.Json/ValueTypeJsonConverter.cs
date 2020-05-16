using System;
using Newtonsoft.Json;

namespace Aggregail.Newtonsoft.Json
{
    /// <summary>
    /// Converts a ValueType object such as a <see langword="struct"/> to and from JSON.
    /// </summary>
    /// <typeparam name="T">The type of value to convert.</typeparam>
    /// <remarks>
    /// This class handles nullability serialization of value types.
    /// </remarks>
    public abstract class ValueTypeJsonConverter<T> : JsonConverter
        where T : struct
    {
        /// <inheritdoc />
        public override bool CanConvert(Type objectType) => 
            objectType == typeof(T) || Nullable.GetUnderlyingType(objectType) == typeof(T);

        /// <inheritdoc />
        public override bool CanWrite => true;

        /// <inheritdoc />
        public override bool CanRead => true;

        /// <inheritdoc />
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

        /// <summary>
        /// Writes the JSON representation of the value.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public abstract void WriteJson(JsonWriter writer, T value, JsonSerializer serializer);

        /// <inheritdoc />
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

        /// <summary>
        /// Reads the JSON representation of the value.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public abstract T ReadJson(JsonReader reader, Type objectType, T? existingValue, JsonSerializer serializer);
    }
}