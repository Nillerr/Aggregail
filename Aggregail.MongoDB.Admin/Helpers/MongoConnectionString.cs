using System;
using MongoDB.Driver;

namespace Aggregail.MongoDB.Admin.Helpers
{
    static internal class MongoConnectionString
    {
        public static string Censored(string connectionString)
        {
            var connectionStringUrl = MongoUrl.Create(connectionString);

            if (!String.IsNullOrEmpty(connectionStringUrl.Password))
            {
                var prefix = $"{connectionStringUrl.Username}:";
                connectionString = connectionString.Replace($"{prefix}{connectionStringUrl.Password}", $"{prefix}<redacted>");
            }

            return connectionString;
        }
    }
}