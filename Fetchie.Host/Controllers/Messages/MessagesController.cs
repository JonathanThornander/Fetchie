using Fetchie.Host.Controllers.Messages.DataTransfer;
using Fetchie.Host.MediatR.Requests.Messages;
using Fetchie.Host.Models;
using Fetchie.Host.Queues;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fetchie.Host.Controllers.Messages
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly MessageQueueManager _queueManager;
        private readonly IMediator _mediator;

        public MessagesController(MessageQueueManager queueManager, IMediator mediator)
        {
            _queueManager = queueManager;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> PostLogMessage([FromBody] CreateMessageRequestBody requestBody)
        {
            var messageId = await _mediator.Send(new SendMessageRequest
            {
                Queue = requestBody.Queue,
                Title = requestBody.Title,
                Body = requestBody.Body,
                Severity = Enum.Parse<MessageSeverity>(requestBody.Severity, ignoreCase: true),
                ExpirationUtc = requestBody.ExpirationUtc,
                Replace = requestBody.Replace,
            });

            return Ok(messageId);
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
