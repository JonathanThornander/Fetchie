using Fetchie.Models;

namespace Fetchie.Controllers.Messages.DataTransfer
{
    public class CreateMessageRequestBody
    {
        public required string Queue { get; init; }

        public required string Title { get; init; }

        public required string Body { get; init; }

        public required string Severity { get; init; }

        public DateTime? ExpirationUtc { get; init; }

        public bool? Replace { get; init; }
    }
}
