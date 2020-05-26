# Aggregail.Testing

Adds the `VerifiableEventStore`, which allows verification of events appended to an aggregate stream.

```c#
var serializer = new JsonEventSerializer(JsonSerializer.CreateDefault());
var eventStore = new VerifiableEventStore(serializer);

var aggregate = Goat.Create("g047");
aggregate.Rename("goatl");
await aggregate.CommitAsync(eventStore);

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
