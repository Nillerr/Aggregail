using System.Threading.Tasks;

namespace Aggregail.MongoDB.Admin.Hubs
{
    public interface IAuthenticationClient
    {
        Task SignedIn();
        Task SignedOut();
    }
}