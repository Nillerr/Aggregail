using System;

namespace Aggregail
{
    /// <summary>
    /// Separates stream names using a <see cref="Separator"/> in the format <c>{id}{separator}{aggregateName}</c>.
    /// </summary>
    public sealed class TokenizedStreamNameResolver : IStreamNameResolver
    {
        /// <summary>
        /// Returns an instance of <see cref="TokenizedStreamNameResolver"/>, with a single dash <c>-</c> as the
        /// separator. 
        /// </summary>
        public static readonly TokenizedStreamNameResolver Default = new TokenizedStreamNameResolver("-");
        
        /// <summary>
        /// Creates an instance of the <see cref="TokenizedStreamNameResolver"/> class.
        /// </summary>
        /// <param name="separator">Token to separate aggregate id and name.</param>
        public TokenizedStreamNameResolver(string separator)
        {
            Separator = separator;
        }

        /// <summary>
        /// The token to separate aggregate id and name.
        /// </summary>
        public string Separator { get; }

        /// <inheritdoc />
        public string Stream<TIdentity, TAggregate>(
            TIdentity id,
            AggregateConfiguration<TIdentity, TAggregate> configuration
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return $"{id}{Separator}{configuration.Name}";
        }

        /// <inheritdoc />
        public TIdentity ParseId<TIdentity, TAggregate>(
            string stream,
            AggregateConfiguration<TIdentity, TAggregate> configuration
        ) where TAggregate : Aggregate<TIdentity, TAggregate>
        {
            var id = stream.Split(Separator, 2)[1];
            return configuration.IdentityParser(id);
        }
    }
}