using System;
using Newtonsoft.Json;

namespace Aggregail.Newtonsoft.Json
{
    /// <summary>
    /// Converts a <see cref="IValueObject{T}"/> <see langword="struct"/> to and from JSON.
    /// </summary>
    /// <typeparam name="TValueObject">The type of value object to convert.</typeparam>
    /// <typeparam name="TValue">The underlying type of value to convert.</typeparam>
    public abstract class ValueObjectConverter<TValueObject, TValue> : ValueTypeJsonConverter<TValueObject>
        where TValueObject : struct, IValueObject<TValue>
    {
        private readonly Func<TValue, TValueObject> _constructor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueObjectConverter{TValueObject,TValue}"/> class, by
        /// specifying a function to construct an instance of the value object.
        /// </summary>
        /// <param name="constructor">A function to construct instances of the value object <typeparamref name="TValueObject"/>.</param>
        protected ValueObjectConverter(Func<TValue, TValueObject> constructor)
        {
            _constructor = constructor;
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, TValueObject value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.Value);
        }

        /// <inheritdoc />
        public override TValueObject ReadJson(
            JsonReader reader,
            Type objectType,
            TValueObject? existingValue,
            JsonSerializer serializer
        )
        {
            var value = serializer.Deserialize<TValue>(reader);
            return _constructor(value!);
        }
    }
}