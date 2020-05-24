using System.Threading.Tasks;

namespace Aggregail.MongoDB.Admin.Hubs
{
    public interface IStreamClient
    {
        Task EventRecorded(object e);
    }
}