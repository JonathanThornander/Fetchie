using MediatR;

namespace Fetchie.Host.MediatR.Requests.Queues
{
    public class ListQueuesRequest : IRequest<IEnumerable<string>>
    {
    }
}
