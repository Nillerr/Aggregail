using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;

namespace Aggregail.MongoDB.Admin.Hubs
{
    public sealed class AuthenticationHub : Hub<IAuthenticationClient>
    {
        public async Task SignIn(string username, string password)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username)
            };
            
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            
            var properties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = false,
                IssuedUtc = DateTimeOffset.UtcNow,
            };
            
            var httpContext = Context.GetHttpContext();
            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                properties
            );

            await Clients.Caller.SignedIn();
        }

        public async Task SignOut()
        {
            var httpContext = Context.GetHttpContext();
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            await Clients.Caller.SignedOut();
        }
    }
}