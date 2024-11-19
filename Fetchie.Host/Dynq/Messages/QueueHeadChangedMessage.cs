using Dynq;
using Fetchie.Host.Models;

namespace Fetchie.Host.Dynq.Messages
{
    public class QueueHeadChangedMessage : IMessage
    {
        public required string Queue { get; init; }

        public required Message? Head { get; init; }
    }
}
