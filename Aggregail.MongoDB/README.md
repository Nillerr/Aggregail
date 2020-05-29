# Aggregail (MongoDB)

A standalone MongoDB Event Store client for [Aggregail](../README.md), using MongoDB as an Event Store.

## Getting Started

Aggregail.MongoDB can be installed using the NuGet Package Manager, or the `dotnet` CLI.
```
dotnet add package Aggregail.MongoDB
```

### MongoDB Replica Set

Aggregail.MongoDB uses MongoDB Transactions to ensure consistency, and thus requires running a MongoDB configuration 
with a replica set, or a sharded cluster. See [docker-compose.yml](docker-compose.yml) for an example configuration 
of a replica set, running with one primary, one secondary and one arbiter node (a PSA configuration).

Aggregail.MongoDB works for [sharded](https://docs.mongodb.com/manual/sharding/) configurations as well.

See [Convert a Standalone to a Replica Set](https://docs.mongodb.com/manual/tutorial/convert-standalone-to-replica-set/) 
for instructions on deploying a replica set to your local `mongod` instance.

### Example

```c#
var connectionString = $"mongodb://root:example@localhost:27017/aggregail?authSource=admin&replicaSet=rs0";
var mongoClient = new MongoClient(connectionString);
var mongoDatabase = mongoClient.GetDatabase("aggregail");

const string collection = "streams";
var serializer = new JsonEventSerializer();
var mongoSettings = new MongoEventStoreSettings(mongoDatabase, collection, serializer);

var mongoStore = new MongoEventStore(mongoSettings);
await mongoStore.InitializeAsync();
```

## Metadata

It is possible to associate metadata with events. At this time Aggregail does not provide a way of reading 
this metadata, other than through the use of [Aggregail.MongoDB.Admin](../Aggregail.MongoDB.Admin). We are 
evaluating workflows involving event metadata, and how to present it to developers.

```c#
class UserIdMetadataFactory : IMetadataFactory
{
    public UserIdMetadataFactory(string userId)
    {
        UserId = userId;
    }

    private string UserId { get; set; }

    public byte[] MetadataFor<T>(Guid id, EventType<T> type, T data, IJsonEventSerializer serializer)
    {
        var metadata = new Metadata(UserId);
        return serializer.Serialize(metadata);
    }

    private class Metadata
    {
        public Metadata(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; }
    }
}

var mongoSettings = new MongoEventStoreSettings(mongoDatabase, collection, serializer);
mongoSettings.MetadataFactory = new UserIdMetadataFactory("nije");
```

## WIP

The following features are being worked on:

 - Supporting soft-deletes of streams, by allowing streams to start from an event other than 0.