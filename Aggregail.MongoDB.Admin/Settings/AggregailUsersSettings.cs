namespace Aggregail.MongoDB.Admin.Settings
{
    public sealed class AggregailUsersSettings
    {
        public string? ConnectionString { get; set; }
        public string? Database { get; set; }
        public string? Collection { get; set; }
    }
}