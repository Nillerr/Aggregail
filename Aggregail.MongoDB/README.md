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

IEventStore mongoStore = new MongoEventStore(mongoSettings);
```
