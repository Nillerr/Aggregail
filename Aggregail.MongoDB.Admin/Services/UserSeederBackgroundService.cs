using System.Threading;
using System.Threading.Tasks;
using Aggregail.MongoDB.Admin.Controllers;
using Aggregail.MongoDB.Admin.Documents;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Aggregail.MongoDB.Admin.Services
{
    public sealed class UserSeederBackgroundService : BackgroundService
    {
        private readonly IMongoCollection<UserDocument> _users;
        private readonly UserDocumentPasswordHasher _passwordHasher;

        public UserSeederBackgroundService(
            IMongoCollection<UserDocument> users,
            UserDocumentPasswordHasher passwordHasher
        )
        {
            _users = users;
            _passwordHasher = passwordHasher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var admin = await _users
                .Find(e => e.Username == "admin")
                .FirstOrDefaultAsync(stoppingToken);

            if (admin != null)
            {
                return;
            }

            admin = new UserDocument
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Username = "admin"
            };

            admin.Password = _passwordHasher.HashPassword(admin, "changeit");

            await _users.InsertOneAsync(admin, cancellationToken: stoppingToken);
        }
    }
}