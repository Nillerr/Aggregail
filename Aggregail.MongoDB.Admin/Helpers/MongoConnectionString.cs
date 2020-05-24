using MongoDB.Driver;

namespace Aggregail.MongoDB.Admin.Helpers
{
    internal static class MongoConnectionString
    {
        public static string Censored(string connectionString)
        {
            var connectionStringUrl = MongoUrl.Create(connectionString);

            if (!string.IsNullOrEmpty(connectionStringUrl.Password))
            {
                connectionString = connectionString
                    .Replace($"{connectionStringUrl.Username}:{connectionStringUrl.Password}", "<hidden>");
            }

            return connectionString;
        }
    }
}