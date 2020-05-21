using System.Security.Claims;
using System.Threading.Tasks;
using Aggregail.MongoDB.Admin.Documents;
using Microsoft.AspNetCore.Authentication.Cookies;
using MongoDB.Driver;

namespace Aggregail.MongoDB.Admin.Authentication
{
    public sealed class UserValidationEvents : CookieAuthenticationEvents
    {
        private readonly IMongoCollection<UserDocument> _users;

        public UserValidationEvents(IMongoCollection<UserDocument> users)
        {
            _users = users;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var principal = context.Principal;

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                context.RejectPrincipal();
                return;
            }

            var username = principal.FindFirstValue(ClaimTypes.Name);
            if (username == null)
            {
                context.RejectPrincipal();
                return;
            }
            
            var passwordVersion = principal.FindFirstValue("pwv");
            if (passwordVersion == null)
            {
                context.RejectPrincipal();
                return;
            }

            var user = await _users
                .Find(e => e.Id == userId)
                .FirstOrDefaultAsync();

            if (user == null || user.Username != username || user.GetPasswordVersion() != passwordVersion)
            {
                context.RejectPrincipal();
            }
        }
    }
}