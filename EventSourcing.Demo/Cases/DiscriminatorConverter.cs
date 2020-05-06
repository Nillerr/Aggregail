using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EventSourcing.Demo.Cases
{
    public sealed class DiscriminatorConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var knownTypesAttribute = objectType.GetCustomAttribute<JsonKnownTypesAttribute>();
            if (knownTypesAttribute == null)
            {
                throw new InvalidOperationException();
            }
            
            var kind = ReadKind(reader, objectType, serializer, knownTypesAttribute);
            var knownType = KnownType(knownTypesAttribute, kind);
            return serializer.Deserialize(reader, knownType);
        }

        private static Type KnownType(JsonKnownTypesAttribute knownTypesAttribute, object kind)
        {
            var knownTypes = knownTypesAttribute.KnownTypes;

            var knownType = (
                from t in knownTypes
                let discriminator = t.GetCustomAttribute<JsonDiscriminatorAttribute>()
                where discriminator != null && discriminator.Discriminator == kind
                select t
            ).First();
            
            return knownType;
        }

        private static object ReadKind(
            JsonReader reader,
            Type objectType,
            JsonSerializer serializer,
            JsonKnownTypesAttribute knownTypesAttribute
        )
        {
            var kindProperty = objectType.GetProperty(knownTypesAttribute.PropertyName);
            if (kindProperty == null)
            {
                throw new InvalidOperationException();
            }
            
            var kindType = kindProperty.PropertyType;
            
            var obj = serializer.Deserialize<JObject>(reader);
            var kindToken = obj[knownTypesAttribute.PropertyName];
            if (kindToken.Type == JTokenType.Null)
            {
                throw new InvalidOperationException();
            }
            
            var kind = kindToken.ToObject(kindType);
            return kind;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.GetCustomAttribute<JsonKnownTypesAttribute>() != null;
        }
    }
}