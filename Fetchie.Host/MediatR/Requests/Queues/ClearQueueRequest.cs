using MediatR;

namespace Fetchie.Host.MediatR.Requests.Queues
{
    public class ClearQueueRequest : IRequest
    {
        public required string Queue { get; init; }
    }
}
