using Fetchie.Host.MediatR.Requests.Queues;
using Fetchie.Host.Models;
using Fetchie.Host.Queues;
using MediatR;

namespace Fetchie.Host.MediatR.Handlers.Queues
{
    public class GetQueueHeadHandler : IRequestHandler<GetQueueHeadRequest, Message?>
    {
        private readonly MessageQueueManager _multiQueueManager;

        public GetQueueHeadHandler(MessageQueueManager multiQueueManager)
        {
            _multiQueueManager = multiQueueManager;
        }

        public Task<Message?> Handle(GetQueueHeadRequest request, CancellationToken cancellationToken)
        {
            var queue = _multiQueueManager.GetQueue(request.Queue);

            return Task.FromResult(queue?.Head);
        }
    }
}
