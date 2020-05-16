using System;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Aggregail.MongoDB
{
    internal sealed class JsonBsonDocumentSerializer : SealedClassSerializerBase<byte[]?>
    {
        protected override byte[]? DeserializeValue(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var document = BsonSerializer.Deserialize<BsonDocument>(context.Reader);
            if (document == null)
            {
                return null;
            }
            
            var json = document.ToJson();
            var bytes = Encoding.UTF8.GetBytes(json);
            return bytes;
        }

        protected override void SerializeValue(BsonSerializationContext context, BsonSerializationArgs args, byte[]? value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
            }
            else
            {
                var json = Encoding.UTF8.GetString(value);
                var document = BsonDocument.Parse(json);
                BsonSerializer.Serialize(context.Writer, document);
            }
        }
    }
}