using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Aggregail.MongoDB.Admin.Documents;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Aggregail.MongoDB.Admin.Controllers
{
    [ApiController]
    [Route("auth")]
    public sealed class AuthenticationController : ControllerBase
    {
        private readonly IMongoCollection<UserDocument> _users;
        private readonly UserDocumentPasswordHasher _passwordHasher;
        private readonly IClock _clock;

        public AuthenticationController(
            IMongoCollection<UserDocument> users,
            UserDocumentPasswordHasher passwordHasher,
            IClock clock
        )
        {
            _users = users;
            _passwordHasher = passwordHasher;
            _clock = clock;
        }

        [HttpPost("login")]
        public async Task<IActionResult> SignIn(string username, string password, CancellationToken cancellationToken)
        {
            var user = await _users
                .Find(e => e.Username == username)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null || !_passwordHasher.VerifyHashedPassword(user, user.Password, password))
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var properties = new AuthenticationProperties
            {
                AllowRefresh = false,
                ExpiresUtc = null,
                IsPersistent = true,
                IssuedUtc = _clock.UtcNow,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                properties
            );

            return Ok();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
    }
}