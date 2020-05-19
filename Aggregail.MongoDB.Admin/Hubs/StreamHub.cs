using Microsoft.AspNetCore.SignalR;

namespace Aggregail.MongoDB.Admin.Hubs
{
    public sealed class StreamHub : Hub<IStreamClient>
    {
    }
}