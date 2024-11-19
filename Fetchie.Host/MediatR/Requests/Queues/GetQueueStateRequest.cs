using Fetchie.Host.Models;
using MediatR;

namespace Fetchie.Host.MediatR.Requests.Queues
{
    public class GetQueueHeadRequest : IRequest<Message?>
    {
        public required string Queue { get; init; }
    }
}
