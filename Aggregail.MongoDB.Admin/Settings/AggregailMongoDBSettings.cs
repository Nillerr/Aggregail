using Aggregail.MongoDB.Admin.Exceptions;

namespace Aggregail.MongoDB.Admin.Settings
{
    public sealed class AggregailMongoDBSettings
    {
        public bool QuietStartup { get; set; } = false;
        public string ConnectionString { get; set; } = null!;
        public string Database { get; set; } = null!;
        public string Collection { get; set; } = null!;
        public AggregailUsersSettings Users { get; set; } = new AggregailUsersSettings();

        public void Validate()
        {
            if (ConnectionString == null)
                throw new AggregailStartupException("Invalid configuration, no connection string was specified.");

            if (Database == null)
                throw new AggregailStartupException("Invalid configuration, no database was specified.");

            if (Collection == null)
                throw new AggregailStartupException("Invalid configuration, no collection was specified.");

            if (Users.Collection == null)
                throw new AggregailStartupException("Invalid configuration, no users collection was specified.");
        }
    }
}