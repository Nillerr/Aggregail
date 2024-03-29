using System.Threading;
using System.Threading.Tasks;
using Aggregail.MongoDB.Admin.Controllers;
using Aggregail.MongoDB.Admin.Documents;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Aggregail.MongoDB.Admin.Services
{
    public sealed class UserBackgroundService : BackgroundService
    {
        private readonly IMongoCollection<UserDocument> _users;
        private readonly UserDocumentPasswordHasher _passwordHasher;

        public UserBackgroundService(
            IMongoCollection<UserDocument> users,
            UserDocumentPasswordHasher passwordHasher
        )
        {
            _users = users;
            _passwordHasher = passwordHasher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await CreateIndexesAsync(stoppingToken);

            var hasAnyUsers = await _users
                .Find(FilterDefinition<UserDocument>.Empty)
                .AnyAsync(stoppingToken);

            if (hasAnyUsers)
            {
                return;
            }

            var id = ObjectId.GenerateNewId().ToString();

            var admin = new UserDocument
            {
                Id = id,
                Username = "admin",
                FullName = "Event Store Administrator",
                Password = _passwordHasher.HashPassword(id, "changeit")
            };
            
            await _users.InsertOneAsync(admin, cancellationToken: stoppingToken);
        }

        private async Task CreateIndexesAsync(CancellationToken cancellationToken)
        {
            var keyBuilder = Builders<UserDocument>.IndexKeys;
            var keys = keyBuilder.Ascending(e => e.Username);

            var options = new CreateIndexOptions();
            options.Unique = true;
            
            var model = new CreateIndexModel<UserDocument>(keys, options);
            await _users.Indexes.CreateOneAsync(model, null, cancellationToken);
        }
    }
}