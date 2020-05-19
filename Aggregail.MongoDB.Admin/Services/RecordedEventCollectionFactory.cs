using Aggregail.MongoDB.Admin.Documents;
using Aggregail.MongoDB.Admin.Hubs;
using Aggregail.MongoDB.Admin.Settings;
using MongoDB.Driver;

namespace Aggregail.MongoDB.Admin.Services
{
    public sealed class RecordedEventCollectionFactory
    {
        public RecordedEventCollectionFactory(MongoCollectionFactory collectionFactory, AggregailSettings settings)
        {
            Collection = collectionFactory.Collection<RecordedEventDocument>(settings.MongoDB.Collection);
        }

        public IMongoCollection<RecordedEventDocument> Collection { get; }
    }
}