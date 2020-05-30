using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Aggregail.Testing
{
    /// <summary>
    /// An event store which records calls to <see cref="AppendToStreamAsync{TIdentity,TAggregate}"/>, for later
    /// verification using <see cref="VerifyAppendToStream{TIdentity,TAggregate}"/>.
    /// </summary>
    /// <remarks>
    /// This class uses an instance of <see cref="InMemoryEventStore"/> as the underlying event store, meaning event
    /// data serialization and deserialization will be tested as well.
    /// </remarks>
    public sealed class VerifiableEventStore : IEventStore
    {
        private readonly IJsonEventSerializer _serializer;
        private readonly InMemoryEventStore _store;

        private readonly List<Append> _appends = new List<Append>();

        private static string ExpectedVersionString(long expectedVersion)
        {
            return expectedVersion == ExpectedVersion.NoStream
                ? nameof(ExpectedVersion) + "." + nameof(ExpectedVersion.NoStream)
                : expectedVersion.ToString("D");
        }

        private sealed class Append
        {
            public Append(object id, Type aggregateType, long expectedVersion, List<IPendingEvent> pendingEvents)
            {
                Id = id;
                AggregateType = aggregateType;
                ExpectedVersion = expectedVersion;
                PendingEvents = pendingEvents;
            }

            public object Id { get; }
            public Type AggregateType { get; }
            public long ExpectedVersion { get; }
            public List<IPendingEvent> PendingEvents { get; }

            public override string ToString()
            {
                var pendingEvents = $"[{string.Join(", ", PendingEvents.Select(e => $"\"{e.Type}\""))}]";

                var expectedVersion = ExpectedVersionString(ExpectedVersion);

                const string method = nameof(IEventStore.AppendToStreamAsync);
                return $"{method}({Id}, <configuration>, {expectedVersion}, {pendingEvents})";
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="VerifiableEventStore"/> class.
        /// </summary>
        /// <param name="settings">Settings</param>
        public VerifiableEventStore(VerifiableEventStoreSettings settings)
        {
            _serializer = settings.EventSerializer;
            
            var inMemorySettings = new InMemoryEventStoreSettings(settings.EventSerializer);
            inMemorySettings.StreamNameResolver = settings.StreamNameResolver;
            
            _store = new InMemoryEventStore(inMemorySettings);
        }

        /// <inheritdoc />
        public Task AppendToStreamAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long expectedVersion,
            IEnumerable<IPendingEvent> pendingEvents,
            CancellationToken cancellationToken = default
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            var events = pendingEvents.ToList();

            var append = new Append(id!, configuration.AggregateType, expectedVersion, events);
            _appends.Add(append);

            return _store.AppendToStreamAsync(id, configuration, expectedVersion, events, cancellationToken);
        }

        /// <inheritdoc />
        public Task<TAggregate?> AggregateAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long? version = null,
            CancellationToken cancellationToken = default
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            return _store.AggregateAsync(id, configuration, version, cancellationToken);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<TIdentity> AggregateIdsAsync<TIdentity, TAggregate>(
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            CancellationToken cancellationToken = default
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            return _store.AggregateIdsAsync(configuration, cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteAggregateAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            long expectedVersion,
            CancellationToken cancellationToken = default
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            return _store.DeleteAggregateAsync(id, configuration, expectedVersion, cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> AggregateExistsAsync<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration,
            CancellationToken cancellationToken = default
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            return _store.AggregateExistsAsync(id, configuration, cancellationToken);
        }

        /// <summary>
        /// Returns the current version of the stream associated with the <paramref name="aggregate"/>.
        /// </summary>
        /// <param name="aggregate">Aggregate to resolve the version for.</param>
        /// <typeparam name="TIdentity">Type of ID of the aggregate.</typeparam>
        /// <typeparam name="TAggregate">Type of aggregate.</typeparam>
        /// <returns>
        /// The current version of the aggregate, as recorded by calls to
        /// <see cref="AppendToStreamAsync{TIdentity,TAggregate}"/>.
        /// </returns>
        public long CurrentVersion<TIdentity, TAggregate>(Aggregate<TIdentity, TAggregate> aggregate)
            where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            return _appends
                .Where(append => append.AggregateType == typeof(TAggregate) && Equals(append.Id, aggregate.Id))
                .Select(append => (long?) append.ExpectedVersion + append.PendingEvents.Count)
                .LastOrDefault() ?? ExpectedVersion.NoStream;
        }

        /// <summary>
        /// Verifies a set of events being appended to the stream.
        /// </summary>
        /// <param name="aggregate">Aggregate to verify events appended to it's stream for.</param>
        /// <param name="expectedVersion">
        /// The expected version used when calling <see cref="AppendToStreamAsync{TIdentity,TAggregate}"/>. See
        /// <see cref="CurrentVersion{TIdentity,TAggregate}"/> for obtaining the current version of a stream.
        /// </param>
        /// <param name="verification">The events to verify and assert.</param>
        /// <typeparam name="TIdentity">Type of ID of the aggregate.</typeparam>
        /// <typeparam name="TAggregate">Type of aggregate.</typeparam>
        /// <exception cref="InvalidOperationException">
        /// No calls to <see cref="AppendToStreamAsync{TIdentity,TAggregate}"/> matching the expected was found.
        /// </exception>
        /// <remarks>
        /// The order of events configured in the <paramref name="verification"/>, must match the expected order of the
        /// events appended by a previous call to <see cref="AppendToStreamAsync{TIdentity,TAggregate}"/>.
        /// </remarks>
        public void VerifyAppendToStream<TIdentity, TAggregate>(
            Aggregate<TIdentity, TAggregate> aggregate,
            long expectedVersion,
            Func<EventAssertionsBuilder, EventAssertionsBuilder> verification
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            var evb = new EventAssertionsBuilder();
            verification(evb);

            var append = _appends
                .FirstOrDefault(app => Matches(typeof(TAggregate), aggregate.Id!, expectedVersion, app, evb));

            if (append == null)
            {
                var ev = ExpectedVersionString(expectedVersion);
                
                throw new InvalidOperationException(
                    $"No append matching the assertions specified was found.{Environment.NewLine}" +
                    $"{Environment.NewLine}" +
                    $"Expected an append matching:{Environment.NewLine}" +
                    $"\tAppendToStreamAsync({aggregate.Id}, <configuration>, {ev}, {evb}){Environment.NewLine}" +
                    $"{Environment.NewLine}" +
                    $"Recorded appends:{Environment.NewLine}" +
                    $"{string.Join(Environment.NewLine, _appends.Select(app => $"\t{app}"))}{Environment.NewLine}"
                );
            }
        }

        private bool Matches(Type aggregateType, object id, long expectedVersion, Append append, EventAssertionsBuilder evb)
        {
            if (append.AggregateType != aggregateType || !Equals(id, append.Id) || expectedVersion != append.ExpectedVersion)
            {
                return false;
            }

            if (append.PendingEvents.Count != evb.Assertions.Count)
            {
                return false;
            }

            for (var i = 0; i < append.PendingEvents.Count; i++)
            {
                var pendingEvent = append.PendingEvents[i];
                var eventAssertion = evb.Assertions[i];
                if (eventAssertion.EventType != pendingEvent.Type)
                {
                    return false;
                }

                eventAssertion.Assertions(pendingEvent, _serializer);
            }

            return true;
        }
    }
}