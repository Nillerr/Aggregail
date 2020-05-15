# Aggregail.NET

> A microframework for working with Aggregate Roots in an Event Sourced system, backed by an Event Store.

Provides connectors for eventstore.com and MongoDB

`Aggregail`
`Aggregail.Newtonsoft.Json`
`Aggregail.MongoDB`
`Aggregail.EventStore`

A practical example

## Create
```c#
class CreateCaseRequest {
    Guid Id { get; set; }
    string Subject { get; set; }
    string Description { get; set; }
}

[HttpPost]
async Task<IActionResult> CreateAsync(CreateCaseRequest request) {
    var @case = Case.Create(request.Id, request.Subject, request.Description);
    await @case.CommitAsync(eventStore);
    return Created();
}
```

 1) `POST /cases`
 2) `Case.Create(...)` validates the input, and creates a new instance of `Case`, by creating a `new CaseCreated(...)` event, 
 appends it to the `Case` Aggregate Root using `Append(event)`, and calls the associated `Apply(CaseCreated)` method on the 
 new `Case` instance.
 3) Calling `@case.CommitAsync(store, id)` saves the `CaseCreated` event in the stream `Case-{id}` as Event `#0`
 4) Respond with `201 Created`

## Import
```c#
async Task ImportAsync(Guid id) {
    var incident = organizationService.RetrieveAsync(id);
    if (incident == null) {
        throw new ArgumentException($"The incident with id {id} could not be found", nameof(id));
    }
    
    var @case = await Case.FromAsync(eventStore, id);
    if (@case == null) {
        // Case is unknown to us
        return;
    }
    
    @case.Import(incident);
    await @case.CommitAsync(eventStore);
}
```
 1) Retrieve `incident` from CRM
 2) Retrieve the matching `Case` from Event Store by replaying every event in the stream to build the current state of 
 the `Case` Aggregate Root.
 3) `@case.Import` creates an `IncidentImported` event instance, appends it to the `Case` Aggregate Root using `Append(event)`, 
 and calls the associated `Apply(IncidentImported)` method.
 4) Calling `@case.CommitAsync(store, id)` saves the pending event (`IncidentImported`) to the stream `Case-{id}` in the 
 Event Store, expecting the stream to already exist at the Version of the latest event applied while constructing the `Case` 
 instance in `Case.FromAsync(eventStore, id)`.

 