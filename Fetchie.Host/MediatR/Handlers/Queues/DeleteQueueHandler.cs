using Fetchie.Host.MediatR.Requests.Queues;
using Fetchie.Host.Queues;
using MediatR;

namespace Fetchie.Host.MediatR.Handlers.Queues
{
    public class DeleteQueueHandler : IRequestHandler<DeleteQueueRequest>
    {
        private readonly MessageQueueManager _multiQueueManager;

        public DeleteQueueHandler(MessageQueueManager multiQueueManager)
        {
            _multiQueueManager = multiQueueManager;
        }

        public Task Handle(DeleteQueueRequest request, CancellationToken cancellationToken)
        {
            _multiQueueManager.DeleteQueue(request.Queue);

            return Task.CompletedTask;
        }
    }
}
