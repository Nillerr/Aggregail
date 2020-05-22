using Aggregail.MongoDB.Admin.Settings;
using MongoDB.Driver;

namespace Aggregail.MongoDB.Admin.Services
{
    public sealed class MongoCollectionFactory
    {
        private readonly IMongoDatabase _database;

        public MongoCollectionFactory(MongoDatabaseFactory databaseFactory, AggregailMongoDBSettings settings)
        {
            _database = databaseFactory.GetDatabase(settings.Database);
        }

        public IMongoCollection<T> Collection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}