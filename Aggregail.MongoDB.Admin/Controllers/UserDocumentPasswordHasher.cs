using System;
using System.Security.Cryptography;
using System.Text;
using Aggregail.MongoDB.Admin.Documents;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Aggregail.MongoDB.Admin.Controllers
{
    public sealed class UserDocumentPasswordHasher
    {
        public string HashPassword(UserDocument user, string password)
        {
            var hashedPassword = HashedPassword(user, password);
            return Convert.ToBase64String(hashedPassword);
        }

        private static byte[] Salt(UserDocument user)
        {
            return Encoding.UTF8.GetBytes(user.Id);
        }

        public bool VerifyHashedPassword(UserDocument user, string hashedPassword, string providedPassword)
        {
            var decodedHashedPassword = Convert.FromBase64String(hashedPassword);
            if (decodedHashedPassword.Length == 0)
            {
                return false;
            }

            var hashedProvidedPassword = HashedPassword(user, providedPassword);
            return CryptographicOperations.FixedTimeEquals(decodedHashedPassword, hashedProvidedPassword);
        }

        private static byte[] HashedPassword(UserDocument user, string password)
        {
            var salt = Encoding.UTF8.GetBytes(user.Id);
            return KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 10_000, 256 / 8);
        }
    }
}