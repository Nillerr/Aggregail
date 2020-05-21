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
    [Route("api/userinfo")]
    public sealed class UserInfoController : ControllerBase
    {
        private readonly IMongoCollection<UserDocument> _users;

        public UserInfoController(IMongoCollection<UserDocument> users)
        {
            _users = users;
        }

        [HttpGet]
        public async Task<ActionResult<User>> UserInfoAsync(CancellationToken cancellationToken)
        {
            var username = User.Identity.Name!;

            var document = await _users
                .Find(e => e.Username == username)
                .Project(e => new {e.Id, e.Username, e.FullName})
                .FirstAsync(cancellationToken);

            var user = new User(document.Id, document.Username, document.FullName);
            return user;
        }
    }
}