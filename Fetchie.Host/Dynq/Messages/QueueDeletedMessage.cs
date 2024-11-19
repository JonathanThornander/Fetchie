using Dynq;

namespace Fetchie.Host.Dynq.Messages
{
    public class QueueDeletedMessage : IMessage
    {
        public required string Queue { get; init; }
    }
}
