using Fetchie.Host.MediatR.Requests.Queues;
using Fetchie.Host.Queues;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fetchie.Host.Controllers.Queues
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueuesController : ControllerBase
    {
        private readonly MessageQueueManager _queueManager;
        private readonly IMediator _mediator;

        public QueuesController(MessageQueueManager queueManager, IMediator mediator)
        {
            _queueManager = queueManager;
            _mediator = mediator;
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListQueues()
        {
            var queues = await _mediator.Send(new ListQueuesRequest());

            return Ok(queues);
        }

        [HttpDelete("{queueName}/delete")]
        public async Task<IActionResult> DeleteQueue(string queueName)
        {
            await _mediator.Send(new DeleteQueueRequest { Queue = queueName });

            return Ok();
        }

        [HttpDelete("{queueName}/clear")]
        public async Task<IActionResult> ClearQueue(string queueName)
        {
            await _mediator.Send(new ClearQueueRequest { Queue = queueName });

            return Ok();
        }

        [HttpGet("{queueName}/head")]
        public async Task<IActionResult> GetQueueHead(string queueName)
        {
            var head = await _mediator.Send(new GetQueueHeadRequest { Queue = queueName });

            return head != null ? Ok(head) : NoContent();
        }
    }
}
