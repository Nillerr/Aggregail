# Aggregail.Testing

Adds Moq extensions for testing using `Mock<IEventStore>`.

```c#
var eventStore = new Mock<IEventStore>();

var aggregate = Goat.Create("g047");
aggregate.Rename("goatl");

var streamVersion = eventStore.SetupAggregate(aggregate);

var retrieved = await Goat.FromAsync(eventStore.Object, aggregate.Id);
retrieved.Rename("meh");
await retrieved.CommitAsync(eventStore.Object);

eventStore.VerifyAppendToStream(aggregate.Id, streamVersion, verify => verify
    .Event(GoatRenamed.EventType, e =>
    {
        Assert.Equal("meh", e.Name);
    })
);
```