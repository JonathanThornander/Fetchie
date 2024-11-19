using Fetchie.Host.MediatR.Requests.Messages;
using Fetchie.Host.Models;
using Fetchie.Host.Queues;
using MediatR;

namespace Fetchie.Host.MediatR.Handlers.Messages
{
    public class SendMessageHandler : IRequestHandler<SendMessageRequest, Guid>
    {
        private readonly MessageQueueManager _multiQueueManager;

        public SendMessageHandler(MessageQueueManager multiQueueManager)
        {
            _multiQueueManager = multiQueueManager;
        }

        public Task<Guid> Handle(SendMessageRequest request, CancellationToken cancellationToken)
        {
            var logMessage = new Message
            {
                MessageId = Guid.NewGuid(),
                Queue = request.Queue,
                Title = request.Title,
                Body = request.Body,
                Severity = request.Severity,
                ExpirationUtc = request.ExpirationUtc,
                CreatedAt = DateTime.UtcNow
            };

            _multiQueueManager.Enqueue(logMessage);

            return Task.FromResult(logMessage.MessageId);
        }
    }
}
