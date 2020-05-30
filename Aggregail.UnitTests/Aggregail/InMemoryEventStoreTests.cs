using System.Threading;
using System.Threading.Tasks;
using Aggregail.Newtonsoft.Json;
using Newtonsoft.Json;
using Xunit;

namespace Aggregail.UnitTests
{
    public class InMemoryEventStoreTests
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        
        private readonly InMemoryEventStore _store;

        public InMemoryEventStoreTests()
        {
            var serializer = new JsonEventSerializer(JsonSerializer.CreateDefault());
            var settings = new InMemoryEventStoreSettings(serializer);
            _store = new InMemoryEventStore(settings);
        }

        [Fact]
        public async Task AppendToStream()
        {
            
        }
    }
}