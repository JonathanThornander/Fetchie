using MediatR;

namespace Fetchie.Host.MediatR.Requests.Queues
{
    public class DeleteQueueRequest : IRequest
    {
        public required string Queue { get; init; }
    }
}
