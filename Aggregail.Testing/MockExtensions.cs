using System;
using System.Text.Json;
using Aggregail.System.Text.Json;
using Moq;

namespace Aggregail.Testing
{
    public static class MockExtensions
    {
        public static void VerifyAppendToStream<TIdentity>(
            this Mock<IEventStore> eventStore,
            TIdentity id,
            long expectedVersion,
            Action<EventVerificationBuilder> verification,
            IJsonEventSerializer serializer = null
        )
        {
            var evb = new EventVerificationBuilder();
            verification(evb);
            
            var actualSerializer = serializer ?? new JsonEventSerializer(new JsonSerializerOptions());
            evb.Verify(id, eventStore, expectedVersion, actualSerializer);
        }
    }
}