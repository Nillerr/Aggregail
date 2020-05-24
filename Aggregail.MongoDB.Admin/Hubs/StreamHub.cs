using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Aggregail.MongoDB.Admin.Hubs
{
    [Authorize]
    public sealed class StreamHub : Hub<IStreamClient>
    {
        public async Task Test()
        {
            await Clients.Caller.EventRecorded(new { Message = "Hello" });
        }
    }
}