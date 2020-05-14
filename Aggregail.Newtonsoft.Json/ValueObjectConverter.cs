using System;
using Newtonsoft.Json;

namespace Aggregail.Newtonsoft.Json
{
    public abstract class ValueObjectConverter<TValueObject, TValue> : ValueTypeJsonConverter<TValueObject>
        where TValueObject : struct, IValueObject<TValue>
    {
        private readonly Func<TValue, TValueObject> _constructor;

        protected ValueObjectConverter(Func<TValue, TValueObject> constructor)
        {
            _constructor = constructor;
        }

        public override void WriteJson(JsonWriter writer, TValueObject value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.Value);
        }

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