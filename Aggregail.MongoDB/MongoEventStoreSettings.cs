using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Aggregail.MongoDB
{
    public sealed class MongoEventStoreSettings
    {
        public MongoEventStoreSettings(IMongoDatabase database, string collection, IJsonEventSerializer eventSerializer)
        {
            Database = database;
            Collection = collection;
            EventSerializer = eventSerializer;
        }

        public IMongoDatabase Database { get; set; }
        public string Collection { get; set; }
        public IJsonEventSerializer EventSerializer { get; set; }
        public ILogger<MongoEventStore>? Logger { get; set; }
        public TransactionOptions TransactionOptions = new TransactionOptions(
            readConcern: ReadConcern.Snapshot,
            readPreference: ReadPreference.Primary,
            writeConcern: WriteConcern.WMajority
        );
    }
}