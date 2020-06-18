using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aggregail
{
    public abstract class AggregateStore<TIdentity, TAggregate> : IAggregateStore<TIdentity, TAggregate>
        where TAggregate : Aggregate<TIdentity, TAggregate>
    {
        private readonly IEventStore _store;

        /// <summary>
        /// The configuration of this aggregate
        /// </summary>
        protected abstract AggregateConfiguration<TIdentity, TAggregate> Configuration { get; }

        protected AggregateStore(IEventStore store)
        {
            _store = store;
        }

        /// <inheritdoc />
        public Task<TAggregate?> AggregateAsync(
            TIdentity id,
            long? version = null,
            CancellationToken cancellationToken = default
        ) =>
            _store.AggregateAsync(id, Configuration, version, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<TIdentity> IdsAsync(CancellationToken cancellationToken = default) =>
            _store.AggregateIdsAsync(Configuration, cancellationToken);

        /// <inheritdoc />
        public Task DeleteAsync(TIdentity id, CancellationToken cancellationToken = default) =>
            _store.DeleteAggregateAsync(id, Configuration, ExpectedVersion.Any, cancellationToken);

        /// <inheritdoc />
        public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken = default) =>
            _store.AggregateExistsAsync(id, Configuration, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IRecordedEvent<TIdentity, TAggregate>> RecordedEventsAsync(
            long start,
            CancellationToken cancellationToken = default
        ) => _store.ReadStreamEventsAsync(Configuration, start, cancellationToken);

        /// <inheritdoc />
        public Task CommitAsync(TAggregate aggregate, CancellationToken cancellationToken = default) =>
            aggregate.CommitAsync(_store, Configuration, cancellationToken);

        /// <inheritdoc />
        public Task DeleteAsync(TAggregate aggregate, CancellationToken cancellationToken = default) =>
            aggregate.DeleteAsync(_store, Configuration, cancellationToken);
    }
}