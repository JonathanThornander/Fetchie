using Fetchie.Queues;
using Microsoft.AspNetCore.Mvc;

namespace Fetchie.Controllers.Queues
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueuesController : ControllerBase
    {
        private readonly MultiQueueManager _queueManager;

        public QueuesController(MultiQueueManager queueManager)
        {
            _queueManager = queueManager;
        }

        [HttpGet("list")]
        public IActionResult ListQueues()
        {
            return Ok(_queueManager.GetQueueNames());
        }

        [HttpDelete("{queueName}/delete")]
        public IActionResult DeleteQueue(string queueName)
        {
            _queueManager.DeleteQueue(queueName);

            return Ok();
        }

        [HttpDelete("{queueName}/clear")]
        public IActionResult ClearQueue(string queueName)
        {
            _queueManager.ClearQueue(queueName);

            return Ok();
        }

        [HttpGet("{queueName}/state")]
        public IActionResult GetQueueState(string queueName)
        {
            var state = _queueManager.GetCurrentState(queueName);

            return state != null ? Ok(state) : NoContent();
        }
    }
}
