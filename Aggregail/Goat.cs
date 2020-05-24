using System;
using System.Threading.Tasks;

namespace Aggregail
{
    internal sealed class GoatCreated
    {
        public static readonly EventType<GoatCreated> EventType = "GoatCreated";

        public GoatCreated(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    internal sealed class GoatRenamed
    {
        public static readonly EventType<GoatRenamed> EventType = "GoatRenamed";

        public GoatRenamed(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    internal readonly struct GoatId : IEquatable<GoatId>
    {
        private readonly Guid _value;

        private GoatId(Guid value) => _value = value;

        public static GoatId Parse(string input) => new GoatId(Guid.Parse(input));
        public static GoatId NewGoatId() => new GoatId(Guid.NewGuid());

        public bool Equals(GoatId other) => _value.Equals(other._value);
        public override bool Equals(object? obj) => obj is GoatId other && Equals(other);

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(GoatId left, GoatId right) => left.Equals(right);
        public static bool operator !=(GoatId left, GoatId right) => !left.Equals(right);

        public override string ToString() => _value.ToString();
    }
    
    internal sealed class Goat : AbstractAggregate<GoatId, Goat>
    {
        static Goat()
        {
            Configuration = new AggregateConfiguration<GoatId, Goat>("Goat", GoatId.Parse)
                .Constructs(GoatCreated.EventType, (id, e) => new Goat(id, e))
                .Applies(GoatRenamed.EventType, (a, e) => a.Apply(e));
        }

        public static Goat CreateGoat(string name) => 
            Create(
                GoatCreated.EventType,
                new GoatCreated(name),
                e => new Goat(GoatId.NewGoatId(), e)
            );

        private Goat(GoatId id, GoatCreated e)
            : base(id)
        {
            Name = e.Name;
        }

        public string Name { get; private set; }

        public void Rename(string name) =>
            Append(GoatRenamed.EventType, new GoatRenamed(name), Apply);

        private void Apply(GoatRenamed e)
        {
            Name = e.Name;
        }
    }

    internal sealed class Program
    {
        public static async Task RunAsync(IEventStore store)
        {
            var goat0 = await Goat.FromAsync(GoatId.NewGoatId(), store);

            var goat = Goat.CreateGoat("MyGoat");
            goat.Rename("MySpaceGoat");

            await goat.CommitAsync(store);
            
            Console.WriteLine($"Name: {goat.Name}");
        }
    }
}