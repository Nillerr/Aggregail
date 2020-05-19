using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Aggregail.MongoDB.Admin.Hubs
{
    [Authorize]
    public sealed class StreamHub : Hub<IStreamClient>
    {
    }
}