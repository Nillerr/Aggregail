using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Aggregail.System.Text.Json
{
    /// <summary>
    /// Converts a <see cref="IValueObject{T}"/> <see langword="struct"/> to and from JSON.
    /// </summary>
    /// <typeparam name="TValueObject">The type of value object to convert.</typeparam>
    /// <typeparam name="TValue">The underlying type of value to convert.</typeparam>
    public sealed class ValueObjectConverter<TValueObject, TValue> : JsonConverter<TValueObject>
        where TValueObject : struct, IValueObject<TValue>
    {
        private readonly Func<TValue, TValueObject> _constructor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueObjectConverter{TValueObject,TValue}"/> class, by
        /// specifying a function to construct an instance of the value object.
        /// </summary>
        /// <param name="constructor">A function to construct instances of the value object <typeparamref name="TValueObject"/>.</param>
        public ValueObjectConverter(Func<TValue, TValueObject> constructor)
        {
            _constructor = constructor;
        }

        /// <inheritdoc />
        public override TValueObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = JsonSerializer.Deserialize<TValue>(ref reader, options);
            var valueObject = _constructor(value);
            return valueObject;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, TValueObject value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }
}