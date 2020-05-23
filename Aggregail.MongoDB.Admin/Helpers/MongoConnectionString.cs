using System;
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
                var prefix = $"{connectionStringUrl.Username}:";
                connectionString = connectionString.Replace($"{prefix}{connectionStringUrl.Password}", $"{prefix}<redacted>");
            }

            return connectionString;
        }
    }
}