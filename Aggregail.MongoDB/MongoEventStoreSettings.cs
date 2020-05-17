using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Aggregail.MongoDB
{
    /// <summary>
    /// Configuration options for the <see cref="MongoEventStore"/> class.
    /// </summary>
    public sealed class MongoEventStoreSettings
    {
        /// <summary>
        /// Creates an instance of the <see cref="MongoEventStoreSettings"/> class.
        /// </summary>
        /// <param name="database">The MongoDB database accessor.</param>
        /// <param name="collection">The name of the MongoDB collection containing event streams.</param>
        /// <param name="eventSerializer">The serializer to use when serializing events.</param>
        public MongoEventStoreSettings(IMongoDatabase database, string collection, IJsonEventSerializer eventSerializer)
        {
            Database = database;
            Collection = collection;
            EventSerializer = eventSerializer;
        }

        /// <summary>
        /// The MongoDB database accessor.
        /// </summary>
        public IMongoDatabase Database { get; set; }
        
        /// <summary>
        /// The name of the MongoDB collection containing event streams.
        /// </summary>
        public string Collection { get; set; }
        
        /// <summary>
        /// The serializer to use when serializing events.
        /// </summary>
        public IJsonEventSerializer EventSerializer { get; set; }

        /// <summary>
        /// The logger for extracting log messages from the driver, or <c>null</c> if no logging is wanted.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>
        /// </remarks>
        public ILogger<MongoEventStore>? Logger { get; set; } = null;
        
        /// <summary>
        /// The options for MongoDB transactions when storing events. It is <b><i>not</i></b> recommended to change the
        /// defaults, as they are already configured for maximum consistency.
        /// </summary>
        public TransactionOptions TransactionOptions { get; set; } = new TransactionOptions(
            readConcern: ReadConcern.Snapshot,
            readPreference: ReadPreference.Primary,
            writeConcern: WriteConcern.WMajority
        );
    }
}