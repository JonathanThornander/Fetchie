using Fetchie.Host.MediatR.Requests.Queues;
using Fetchie.Host.Queues;
using MediatR;

namespace Fetchie.Host.MediatR.Handlers.Queues
{
    public class ClearQueueHandler : IRequestHandler<ClearQueueRequest>
    {
        private readonly MessageQueueManager _multiQueueManager;

        public ClearQueueHandler(MessageQueueManager multiQueueManager)
        {
            _multiQueueManager = multiQueueManager;
        }

        public Task Handle(ClearQueueRequest request, CancellationToken cancellationToken)
        {
            _multiQueueManager.GetQueue(request.Queue)?.Clear();

            return Task.CompletedTask;
        }
    }
}
