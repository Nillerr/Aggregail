using Aggregail.MongoDB.Admin.Documents;
using Aggregail.MongoDB.Admin.Settings;
using MongoDB.Driver;

namespace Aggregail.MongoDB.Admin.Services
{
    public sealed class RecordedEventCollectionFactory
    {
        public RecordedEventCollectionFactory(MongoCollectionFactory collectionFactory, AggregailMongoDBSettings settings)
        {
            Collection = collectionFactory.Collection<RecordedEventDocument>(settings.Collection);
        }

        public IMongoCollection<RecordedEventDocument> Collection { get; }
    }
}