using System;
using System.Threading.Tasks;
using Aggregail.Newtonsoft.Json;
using Aggregail.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Aggregail.UnitTests
{
    public class VerifiableEventStoreTests
    {
        [Fact]
        public async Task CreateWithNoStream()
        {
            var serializer = new JsonEventSerializer(JsonSerializer.CreateDefault());
            var settings = new VerifiableEventStoreSettings(serializer);
            var eventStore = new VerifiableEventStore(settings);
            
            var aggregate = Goat.Create("g047");
            await aggregate.CommitAsync(eventStore);
            
            eventStore.VerifyAppendToStream(aggregate, ExpectedVersion.NoStream, verify => verify
                .Event(GoatCreated.EventType, e =>
                {
                    Assert.Equal("g047", e.Name);
                })
            );
        }
        
        [Fact]
        public async Task SetupAggregate_VerifyAppendToStream()
        {
            var serializer = new JsonEventSerializer(JsonSerializer.CreateDefault());
            var settings = new VerifiableEventStoreSettings(serializer);
            var eventStore = new VerifiableEventStore(settings);
            
            var aggregate = Goat.Create("g047");
            aggregate.Rename("goatl");
            await aggregate.CommitAsync(eventStore);
            
            eventStore.VerifyAppendToStream(aggregate, ExpectedVersion.NoStream, verify => verify
                .Event(GoatCreated.EventType, e =>
                {
                    Assert.Equal("g047", e.Name);
                })
                .Event(GoatRenamed.EventType, e =>
                {
                    Assert.Equal("goatl", e.Name);
                })
            );

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
        }

        private class GoatCreated
        {
            public static readonly EventType<GoatCreated> EventType = "GoatCreated";
            
            public string Name { get; }

            public GoatCreated(string name)
            {
                Name = name;
            }
        }

        private class GoatRenamed
        {
            public static readonly EventType<GoatRenamed> EventType = "GoatRenamed";
            
            public string Name { get; }

            public GoatRenamed(string name)
            {
                Name = name;
            }
        }

        private class Goat : Aggregate<Guid, Goat>
        {
            private static readonly AggregateConfiguration<Guid, Goat> Configuration =
                new AggregateConfiguration<Guid, Goat>("Goat", Guid.Parse)
                    .Constructs(GoatCreated.EventType, (id, e) => new Goat(id, e))
                    .Applies(GoatRenamed.EventType, (a, e) => a.Apply(e));
            
            public static Goat Create(string name)
            {
                var e = new GoatCreated(name);
                var a = new Goat(Guid.NewGuid(), e);
                a.Append(Guid.NewGuid(), GoatCreated.EventType, e);
                return a;
            }

            public static Task<Goat> FromAsync(IEventStore store, Guid id)
            {
                return store.AggregateAsync(id, Configuration);
            }
            
            public Goat(Guid id, GoatCreated e)
                : base(id)
            {
                Name = e.Name;
            }
            
            public string Name { get; private set; }

            public async Task CommitAsync(IEventStore store)
            {
                await CommitAsync(store, Configuration);
            }

            public void Rename(string name)
            {
                var e = new GoatRenamed(name);
                Apply(e);
                Append(Guid.NewGuid(), GoatRenamed.EventType, e);
            }

            private void Apply(GoatRenamed e)
            {
                Name = e.Name;
            }
        }
    }
}