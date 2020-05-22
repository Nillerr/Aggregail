using Aggregail.MongoDB.Admin.Helpers;
using Aggregail.MongoDB.Admin.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Aggregail.MongoDB.Admin.Documents
{
    public sealed class UserDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("username")]
        public string Username { get; set; } = null!;

        [BsonElement("full_name")]
        public string FullName { get; set; } = null!;

        [BsonElement("password")]
        public string Password { get; set; } = null!;

        public string GetPasswordVersion() => SHA256.ComputeHash(Password);
    }
}