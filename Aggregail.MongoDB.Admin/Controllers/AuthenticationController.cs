using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Aggregail.MongoDB.Admin.Documents;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Aggregail.MongoDB.Admin.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthenticationController : ControllerBase
    {
        private readonly IMongoCollection<UserDocument> _users;
        private readonly UserDocumentPasswordHasher _passwordHasher;
        private readonly ISystemClock _clock;

        public AuthenticationController(
            IMongoCollection<UserDocument> users,
            UserDocumentPasswordHasher passwordHasher,
            ISystemClock clock
        )
        {
            _users = users;
            _passwordHasher = passwordHasher;
            _clock = clock;
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(User.Claims.ToDictionary(e => e.Type, e => e.Value));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> SignInAsync(
            [FromForm] string username,
            [FromForm] string password,
            CancellationToken cancellationToken
        )
        {
            var user = await _users
                .Find(e => e.Username == username)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null || !_passwordHasher.VerifyHashedPassword(user.Id, user.Password, password))
            {
                return Unauthorized();
            }

            var pwv = user.GetPasswordVersion()
                .ToString();
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, username),
                new Claim("pwv", pwv)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var properties = new AuthenticationProperties
            {
                AllowRefresh = false,
                ExpiresUtc = DateTimeOffset.MaxValue,
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

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> SignOutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
    }
}