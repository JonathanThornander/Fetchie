using Fetchie.Host.MediatR.Requests.Queues;
using Fetchie.Host.Queues;
using MediatR;

namespace Fetchie.Host.MediatR.Handlers.Queues
{
    internal class ListQueuesHandler : IRequestHandler<ListQueuesRequest, IEnumerable<string>>
    {
        private readonly MessageQueueManager _multiQueueManager;

        public ListQueuesHandler(MessageQueueManager multiQueueManager)
        {
            _multiQueueManager = multiQueueManager;
        }

        public Task<IEnumerable<string>> Handle(ListQueuesRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_multiQueueManager.GetQueueNames());
        }
    }
}
