using System;
using System.Threading.Tasks;
using Aggregail.Testing;
using Moq;
using Xunit;

namespace Aggregail.UnitTests
{
    public class MockEventStore
    {
        [Fact]
        public async Task CreateWithNoStream()
        {
            var eventStore = new Mock<IEventStore>();
            
            var aggregate = Goat.Create("g047");
            await aggregate.CommitAsync(eventStore.Object);
            
            eventStore.VerifyAppendToStream(aggregate.Id, ExpectedVersion.NoStream, verify => verify
                .Event(GoatCreated.EventType, e =>
                {
                    Assert.Equal("g047", e.Name);
                })
            );
        }
        
        [Fact]
        public async Task SetupAggregate()
        {
            var eventStore = new Mock<IEventStore>();
            
            var aggregate = Goat.Create("g047");
            aggregate.Rename("goatl");

            eventStore.SetupAggregate(aggregate);
            
            var actual = await Goat.FromAsync(eventStore.Object, aggregate.Id);
            Assert.Equal(aggregate.Id, actual.Id);
            Assert.Equal(aggregate.Name, actual.Name);
        }
        
        [Fact]
        public async Task SetupAggregate_VerifyAppendToStream()
        {
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