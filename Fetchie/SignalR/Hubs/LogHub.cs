using Microsoft.AspNetCore.SignalR;

namespace Fetchie.SignalR.Hubs
{
    public class LogHub : Hub
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
