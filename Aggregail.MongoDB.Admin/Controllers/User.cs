namespace Aggregail.MongoDB.Admin.Controllers
{
    public sealed class User
    {
        public string Id { get; }
        public string Username { get; }
        public string FullName { get; }

        public User(string id, string username, string fullName)
        {
            Id = id;
            Username = username;
            FullName = fullName;
        }
    }
}