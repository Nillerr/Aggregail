using Aggregail.MongoDB.Admin.Settings;
using MongoDB.Driver;

namespace Aggregail.MongoDB.Admin.Services
{
    public sealed class MongoDatabaseFactory
    {
        private readonly IMongoClient _client;

        public MongoDatabaseFactory(AggregailMongoDBSettings settings)
        {
            _client = new MongoClient(settings.ConnectionString);
        }

        public IMongoDatabase GetDatabase(string name)
        {
            return _client.GetDatabase(name);
        }
    }
}