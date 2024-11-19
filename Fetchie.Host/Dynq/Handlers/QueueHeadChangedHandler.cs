using Dynq;
using Fetchie.Host.Dynq.Messages;
using Fetchie.Host.SignalR;
using Fetchie.Host.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Fetchie.Host.Dynq.Handlers
{
    public class QueueHeadChangedHandler : IDynqListner<QueueHeadChangedMessage>
    {
        private readonly IHubContext<QueueHub> _hubContext;

        public QueueHeadChangedHandler(IHubContext<QueueHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task HandleMessage(QueueHeadChangedMessage message)
        {
            await _hubContext.Clients.Group(message.Queue).SendAsync(Topics.QueueStateChanged, message.Queue, message.Head);
        }
    }
}
