using Aggregail.MongoDB.Admin.Settings;
using MongoDB.Driver;

namespace Aggregail.MongoDB.Admin.Services
{
    public sealed class MongoDatabaseFactory
    {
        private readonly IMongoClient _client;

        public MongoDatabaseFactory(AggregailSettings settings)
        {
            _client = new MongoClient(settings.MongoDB.ConnectionString);
        }

        public IMongoDatabase GetDatabase(string name)
        {
            return _client.GetDatabase(name);
        }
    }
}