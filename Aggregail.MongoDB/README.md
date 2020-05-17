# Aggregail.NET for MongoDB

```c#
var serializer = new JsonEventSerializer();

var mongoClient = new MongoClient("mongodb://root:example@localhost");
var mongoDatabase = mongoClient.GetDatabase("aggregail");

const string collection = "streams";
IEventStore mongoStore = new MongoEventStore(mongoDatabase, collection, serializer);
```