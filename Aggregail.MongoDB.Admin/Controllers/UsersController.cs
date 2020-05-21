using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aggregail.MongoDB.Admin.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Aggregail.MongoDB.Admin.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public sealed class UsersController : ControllerBase
    {
        private readonly IMongoCollection<UserDocument> _users;

        public UsersController(IMongoCollection<UserDocument> users)
        {
            _users = users;
        }

        [HttpGet]
        public async Task<User[]> UsersAsync(CancellationToken cancellationToken)
        {
            var documents = await _users
                .Find(FilterDefinition<UserDocument>.Empty)
                .ToListAsync(cancellationToken);

            var users = documents
                .Select(document => new User(document.Id, document.Username, document.FullName))
                .ToArray();

            return users;
        }
    }
}