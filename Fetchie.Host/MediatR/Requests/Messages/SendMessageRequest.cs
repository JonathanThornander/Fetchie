using Fetchie.Host.Models;
using MediatR;

namespace Fetchie.Host.MediatR.Requests.Messages
{
    public class SendMessageRequest : IRequest<Guid>
    {
        public required string Queue { get; init; }

        public required string Title { get; init; }

        public required string Body { get; init; }

        public required MessageSeverity Severity { get; init; }

        public DateTime? ExpirationUtc { get; init; }

        public bool? Replace { get; init; }
    }
}
