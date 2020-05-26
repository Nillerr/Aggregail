# Aggregail.Testing

Adds the `VerifiableEventStore`, which allows verification of events appended to an aggregate stream.

```c#
var eventStore = new VerifiableEventStore();

var aggregate = Goat.Create("g047");
aggregate.Rename("goatl");

var streamVersion = eventStore.CurrentVersion(aggregate);

var retrieved = await Goat.FromAsync(eventStore.Object, aggregate.Id);
retrieved.Rename("meh");
await retrieved.CommitAsync(eventStore.Object);

eventStore.VerifyAppendToStream(aggregate, streamVersion, verify => verify
    .Event(GoatRenamed.EventType, e =>
    {
        Assert.Equal("meh", e.Name);
    })
);
```