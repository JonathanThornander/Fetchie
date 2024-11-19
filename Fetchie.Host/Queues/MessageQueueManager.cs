using Dynq;
using Fetchie.Host.Dynq.Messages;
using Fetchie.Host.Models;
using System.Collections.Concurrent;

namespace Fetchie.Host.Queues
{
    public class MessageQueueManager
    {
        private readonly IDynqService _dynqService;

        public MessageQueueManager(IDynqService dynqService)
        {
            _dynqService = dynqService;
        }

        private readonly ConcurrentDictionary<string, MessageQueue> _queues = new();

        public void Enqueue(Message message, bool replace = false)
        {
            var queue = _queues.GetOrAdd(message.Queue, _ => new MessageQueue(message.Queue, _dynqService));

            queue.Enqueue(message, replace);
        }

        public MessageQueue? GetQueue(string queueName)
        {
            _queues.TryGetValue(queueName, out var queue);
            return queue;
        }

        public IEnumerable<string> GetQueueNames() => _queues.Keys;

        public async void DeleteQueue(string queueName)
        {
            if (_queues.TryRemove(queueName, out var _))
            {
                await _dynqService.BroadcastAsync(new QueueDeletedMessage { Queue = queueName });
            }
        }
    }
}
