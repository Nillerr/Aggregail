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

        [HttpGet("{id}")]
        public async Task<User?> UserAsync(string id, CancellationToken cancellationToken)
        {
            var document = await _users
                .Find(e => e.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            if (document == null)
            {
                return null;
            }
            
            var user = new User(document.Id, document.Username, document.FullName);
            return user;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            [FromForm] string username,
            [FromForm] string fullName,
            [FromForm] string password,
            [FromForm] string confirmedPassword,
            CancellationToken cancellationToken
        )
        {
            if (password != confirmedPassword)
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

        [HttpPost("{id}")]
        public async Task<IActionResult> EditAsync(
            string id,
            [FromForm] string fullName,
            CancellationToken cancellationToken
        )
        {
            var filter = Builders<UserDocument>.Filter.Where(e => e.Id == id);
            var update = Builders<UserDocument>.Update.Set(e => e.FullName, fullName);
            var document = await _users.FindOneAndUpdateAsync(filter, update, cancellationToken: cancellationToken);
            if (document == null)
            {
                return NotFound();
            }
            
            return Ok();
        }

        [HttpPost("{id}/password")]
        public async Task<IActionResult> ChangePasswordAsync(
            string id,
            [FromForm] string password,
            [FromForm] string confirmedPassword,
            CancellationToken cancellationToken
        )
        {
            if (password != confirmedPassword)
            {
                return BadRequest();
            }
            
            var filter = Builders<UserDocument>.Filter.Where(e => e.Id == id);
            var update = Builders<UserDocument>.Update.Set(e => e.Password, password);
            var document = await _users.FindOneAndUpdateAsync(filter, update, cancellationToken: cancellationToken);
            if (document == null)
            {
                return NotFound();
            }
            
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            await _users.DeleteOneAsync(e => e.Id == id, cancellationToken);
            return Ok();
        }
    }
}