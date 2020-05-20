using System;
using System.Security.Cryptography;
using System.Text;
using Aggregail.MongoDB.Admin.Documents;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Aggregail.MongoDB.Admin.Controllers
{
    public sealed class UserDocumentPasswordHasher
    {
        public string HashPassword(string userId, string password)
        {
            var hashedPassword = HashedPassword(userId, password);
            return Convert.ToBase64String(hashedPassword);
        }

        private static byte[] Salt(string userId)
        {
            return Encoding.UTF8.GetBytes(userId);
        }

        public bool VerifyHashedPassword(string userId, string hashedPassword, string providedPassword)
        {
            var decodedHashedPassword = Convert.FromBase64String(hashedPassword);
            if (decodedHashedPassword.Length == 0)
            {
                return false;
            }

            var hashedProvidedPassword = HashedPassword(userId, providedPassword);
            return CryptographicOperations.FixedTimeEquals(decodedHashedPassword, hashedProvidedPassword);
        }

        private static byte[] HashedPassword(string userId, string password)
        {
            var salt = Encoding.UTF8.GetBytes(userId);
            return KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 10_000, 256 / 8);
        }
    }
}