using Microsoft.AspNetCore.SignalR;

namespace Fetchie.Host.SignalR.Hubs
{
    public class QueueHub : Hub
    {
        public async Task JoinQueue(string queueName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, queueName);
        }

        public async Task LeaveQueue(string queueName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, queueName);
        }
    }
}
