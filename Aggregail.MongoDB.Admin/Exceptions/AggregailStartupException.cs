using System;

namespace Aggregail.MongoDB.Admin.Exceptions
{
    public sealed class AggregailStartupException : Exception
    {
        public AggregailStartupException(string message) : base(message)
        {
        }
    }
}