using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aggregail.MongoDB.Admin.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Aggregail.MongoDB.Admin.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public sealed class UsersController : ControllerBase
    {
        private readonly IMongoCollection<UserDocument> _users;
        private readonly UserDocumentPasswordHasher _passwordHasher;

        public UsersController(IMongoCollection<UserDocument> users, UserDocumentPasswordHasher passwordHasher)
        {
            _users = users;
            _passwordHasher = passwordHasher;
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

        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            [FromForm] string username,
            [FromForm] string fullName,
            [FromForm] string password,
            [FromForm] string confirmPassword,
            CancellationToken cancellationToken
        )
        {
            if (password != confirmPassword)
            {
                return BadRequest();
            }
            
            var id = ObjectId.GenerateNewId().ToString();
            
            var document = new UserDocument
            {
                Id = id,
                Username = username,
                Password = _passwordHasher.HashPassword(id, password),
                FullName = fullName
            };

            await _users.InsertOneAsync(document, null, cancellationToken);
            return Ok();
        }
    }
}