using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;

namespace Aggregail.MongoDB.Admin.Authentication
{
    public sealed class JsonWebTokenDataFormat : ISecureDataFormat<AuthenticationTicket?>
    {
        private const string Issuer = "https://localhost:5001";
        private const string Audience = "32a507e56d374cd6818d2f7fe8483a1b";
        
        private readonly ISystemClock _clock;

        public JsonWebTokenDataFormat(ISystemClock clock)
        {
            _clock = clock;
            
            const string signingKey =
                "pMGYhe4dnE17NwaNN5fxbfgJ0Wv3px3xIwuXwYiua87e1rbULL" +
                "pMGYhe4dnE17NwaNN5fxbfgJ0Wv3px3xIwuXwYiua87e1rbULLM9H3EBvFWXfb2yp0KTHaT7Qu8ies0go24PVG7741uAYWw237Wa";

            var signingKeyBytes = Encoding.UTF8.GetBytes(signingKey);
            SigningKey = new SymmetricSecurityKey(signingKeyBytes);

            SigningCredentials = new SigningCredentials(
                key: SigningKey,
                algorithm: SecurityAlgorithms.HmacSha256Signature,
                digest: SecurityAlgorithms.Sha256Digest
            );
        }

        private SymmetricSecurityKey SigningKey { get; }
        private SigningCredentials SigningCredentials { get; }

        public string Protect(AuthenticationTicket? data)
        {
            return Protect(data, purpose: null);
        }

        public string Protect(AuthenticationTicket? data, string? purpose)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            
            var handler = new JwtSecurityTokenHandler();
            var subject = new ClaimsIdentity(data.Principal.Identity);

            var now = _clock.UtcNow.UtcDateTime;
            var expires = now + TimeSpan.FromDays(30);
            
            var jwt = handler.CreateEncodedJwt(Issuer, Audience, subject, now, expires, now, SigningCredentials);
            return jwt;
        }

        public AuthenticationTicket? Unprotect(string? protectedText)
        {
            return Unprotect(protectedText, purpose: null);
        }

        public AuthenticationTicket? Unprotect(string? protectedText, string? purpose)
        {
            if (protectedText == null)
            {
                return null;
            }
            
            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                
                ValidIssuer = Issuer,
                ValidateIssuer = true,
                
                ValidAudience = Audience,
                ValidateAudience = true,
                
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = SigningKey,
            };
            
            try
            {
                var principal = handler.ValidateToken(protectedText, validationParameters, out var validatedToken);

                var properties = new AuthenticationProperties
                {
                    ExpiresUtc = validatedToken.ValidTo,
                    IssuedUtc = validatedToken.ValidFrom
                };

                const string authenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                return new AuthenticationTicket(principal, properties, authenticationScheme);
            }
            catch
            {
                return null;
            }
            
        }
    }
}