using Fetchie.Controllers.Messages.DataTransfer;
using Fetchie.Models;
using Fetchie.Queues;
using Microsoft.AspNetCore.Mvc;

namespace Fetchie.Controllers.Messages
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly MultiQueueManager _queueManager;

        public MessagesController(MultiQueueManager queueManager)
        {
            _queueManager = queueManager;
        }

        [HttpPost]
        public IActionResult PostLogMessage([FromBody] CreateMessageRequestBody requestBody)
        {
            var logMessage = new Message
            {
                MessageId = Guid.NewGuid(),
                Queue = requestBody.Queue,
                Title = requestBody.Title,
                Body = requestBody.Body,
                Severity = Enum.Parse<Severity>(requestBody.Severity, ignoreCase: true),
                ExpirationUtc = requestBody.ExpirationUtc,
                CreatedAt = DateTime.UtcNow
            };

            _queueManager.Enqueue(logMessage, replace: requestBody.Replace ?? false);

            return Ok();
        }

        [HttpDelete("{messageId}")]
        public IActionResult DeleteMessage(Guid messageId)
        {
            foreach (var queue in _queueManager.GetQueueNames())
            {
                var logQueue = _queueManager.GetQueue(queue);

                if (logQueue != null)
                {
                    logQueue.Remove(messageId);
                }
            }

            return Ok();
        }
    }
}
