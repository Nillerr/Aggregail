using System.Text;

namespace Aggregail.MongoDB.Admin.Helpers
{
    public static class SHA256
    {
        public static string ComputeHash(string source)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            
            var sourceBytes = Encoding.UTF8.GetBytes(source);
            var hashBytes = sha.ComputeHash(sourceBytes);
            var hash = Encoding.UTF8.GetString(hashBytes);
            return hash;
        }
    }
}