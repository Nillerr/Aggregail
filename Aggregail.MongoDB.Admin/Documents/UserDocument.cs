using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Aggregail.MongoDB.Admin.Documents
{
    public sealed class UserDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; } = null!;

        [BsonElement("password")]
        public string Password { get; set; } = null!;
    }
}