# Aggregail.Testing

Adds the `VerifiableEventStore`, which allows verification of events appended to an aggregate stream.

```c#
var serializer = new JsonEventSerializer(JsonSerializer.CreateDefault());
var settings = new VerifiableEventStoreSettings(serializer);
var eventStore = new VerifiableEventStore(settings);

var aggregate = Goat.Create("g047");
aggregate.Rename("goatl");
await aggregate.CommitAsync(eventStore);

// streamVersion = 1: Create (#0), Rename (#1)
var streamVersion = eventStore.CurrentVersion(aggregate);

var retrieved = await Goat.FromAsync(eventStore, aggregate.Id);
retrieved.Rename("meh");
await retrieved.CommitAsync(eventStore);

eventStore.VerifyAppendToStream(aggregate, streamVersion, verify => verify
    .Event(GoatRenamed.EventType, e =>
    {
        Assert.Equal("meh", e.Name);
    })
);
```
