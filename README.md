# Aggregail

A small framework for implementing Aggregate Roots, which are a critical part of Event Sourcing, DDD and CQRS, backed by an event store.

Provides connectors for [Event Store](https://www.eventstore.com) and MongoDB.

| Package                                                            | NuGet                                                                                                                             |                                                                                                                                       |
|--------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------|
| Aggregail                                                          | [![NuGet](https://img.shields.io/nuget/v/Aggregail)](https://www.nuget.org/packages/Aggregail)                                    | [![Download](https://img.shields.io/nuget/dt/Aggregail)](https://www.nuget.org/packages/Aggregail)                                    |
| [Aggregail.Newtonsoft.Json](Aggregail.Newtonsoft.Json/README.md)   | [![NuGet](https://img.shields.io/nuget/v/Aggregail.Newtonsoft.Json)](https://www.nuget.org/packages/Aggregail.Newtonsoft.Json)    | [![Download](https://img.shields.io/nuget/dt/Aggregail.Newtonsoft.Json)](https://www.nuget.org/packages/Aggregail.Newtonsoft.Json)    |
| [Aggregail.System.Text.Json](Aggregail.System.Text.Json/README.md) | [![NuGet](https://img.shields.io/nuget/v/Aggregail.System.Text.Json)](https://www.nuget.org/packages/Aggregail.System.Text.Json)  | [![Download](https://img.shields.io/nuget/dt/Aggregail.System.Text.Json)](https://www.nuget.org/packages/Aggregail.System.Text.Json)  |
| [Aggregail.MongoDB](Aggregail.MongoDB/README.md)                   | [![NuGet](https://img.shields.io/nuget/v/Aggregail.MongoDB)](https://www.nuget.org/packages/Aggregail.MongoDB)                    | [![Download](https://img.shields.io/nuget/dt/Aggregail.MongoDB)](https://www.nuget.org/packages/Aggregail.MongoDB)                    |
| Aggregail.EventStore                                               | WIP                                                                                                                               |                                                                                                                                       |

A practical example

## Goat

The aggregate root in our example will be a goat. Every aggregate needs at least one event to create it, 
let's call that event `GoatCreated`:

```c#
public class GoatCreated
{
    public static readonly EventType<GoatCreated> EventType = "GoatCreated";

    public string Name { get; set; }
}
```

It wouldn't be much of an example if we could not update the Aggregate Root after creation, so let's 
have an event for that `GoatUpdated`:

```c#
public class GoatUpdated
{
    public static readonly EventType<GoatUpdated> EventType = "GoatUpdated";

    public string Name { get; set; }
}
```

Our goat itself is an example of the minimum amount of code required to make a somewhat interesting 
Aggregate Root:

```c#
public class Goat : Aggregate<Guid, Goat>
{
    private static readonly AggregateConfiguration<Guid, Goat> Configuration =
        new AggregateConfiguration<Guid, Goat>("goat")
            .Constructs(GoatCreated.EventType, (id, e) => new Goat(id, e))
            .Applies(GoatUpdated.EventType, (goat, e) => goat.Apply(e));

    public string Name { get; private set; }

    private Goat(Guid id, GoatCreated e) : base(id)
    {
        Name = e.Name;
    }

    public static Task<Goat> FromAsync(IEventStore store, Guid id) =>
        store.AggregateAsync(id, Configuration);

    public Task CommitAsync(IEventStore store) =>
        CommitAsync(store, Configuration);

    public void Update(string name)
    {
        var e = new GoatUpdated { Name = name };
        Apply(e);
        Append(Guid.NewGuid(), GoatUpdated.EventType, e);
    }

    private void Apply(GoatUpdated e)
    {
        Name = e.Name;
    }
}
```

### Commands

The code above defines two _commands_ and two _events_, and configures the applicator 
(or constructor) for each event type for the Aggregate Root `Goat`.

| Command                                    | Event         | Applicator                     |
|--------------------------------------------|---------------|--------------------------------|
| `static Goat Create(Guid id, string name)` | `GoatCreated` | `Goat(Guid id, GoatCreated e)` |
| `void Update(string name)`                 | `GoatUpdated` | `void Apply(GoatUpdated e)`    |

### Create

```c#
public class CreateGoatRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

[Route("goats")]
public class GoatsController
{
    private readonly IEventStore _store;

    public GoatsController(IEventStore store)
    {
        _store = store;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateGoatRequest request)
    {
        var goat = Goat.Create(request.Id, request.Name);
        await gota.CommitAsync(_store);
        return Created();
    }
}
```

 1) `POST /cases`
 2) `Case.Create(...)` validates the input, and creates a new instance of `Case`, by creating a `new CaseCreated(...)` event, 
 appends it to the `Case` Aggregate Root using `Append(event)`, and calls the associated `Apply(CaseCreated)` method on the 
 new `Case` instance.
 3) Calling `@case.CommitAsync(store, id)` saves the `CaseCreated` event in the stream `Case-{id}` as Event `#0`
 4) Respond with `201 Created`

### Fetch

```c#
[Route("goats")]
public class GoatsController
{
    private readonly IEventStore _store;

    public GoatsController(IEventStore store)
    {
        _store = store;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var goat = await Goat.FromAsync(_store, id);
        if (goat == null)
        {
            return NotFound();
        }
        
        return Ok(goat);
    }
}
```

### Update
```c#
public class UpdateGoatRequest
{
    public string Name { get; set; }
}

[Route("goats")]
public class GoatsController
{
    private readonly IEventStore _store;

    public GoatsController(IEventStore store)
    {
        _store = store;
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateGoatRequest request)
    {
        var goat = await Goat.FromAsync(_store, id);
        if (goat == null)
        {
            return NotFound();
        }
        
        goat.Update(request.Name);
        await goat.CommitAsync(_store);
        return Ok();
    }
}
```

### Delete

Deleting Aggregate Roots, and thus streams is currently not supported by Aggregail, but is actively being worked upon. We 
know it is a vital part in supporting several use cases due to GDPR. If using MongoDB as an event store, streams can 
easily be deleted without the use of Aggregail:

```c#
db.aggregail.deleteMany({ stream: "goat-a4a4e832-f577-4461-a50c-d9c83342ee6f" }) 
```

Likewise, when using Event Store, the streams can be deleted using the `IEventStoreConnection`, or using the UI.

## Concepts

### Aggregate (Root)

#### Events

#### Commands

#### Configuration

##### Constructor

###### Applicator
