using Fetchie.Models;
using Fetchie.SignalR;
using Fetchie.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Fetchie.Queues
{
    public class MultiQueueManager
    {
        private readonly IHubContext<QueueHub> _hubContext;

        public MultiQueueManager(IHubContext<QueueHub> hubContext)
        {
            _hubContext = hubContext;
        }

        private readonly ConcurrentDictionary<string, PriorityMessageQueue> _queues = new();

        public void Enqueue(Message message, bool replace = false)
        {
            var queue = _queues.GetOrAdd(message.Queue, _ => CreateQueue(message.Queue));

            queue.Enqueue(message, replace);
        }

        public Message? GetCurrentState(string queueName)
        {
            if (_queues.TryGetValue(queueName, out var queue))
            {
                return queue.GetCurrentState();
            }

            return null;
        }

        public PriorityMessageQueue? GetQueue(string queueName)
        {
            _queues.TryGetValue(queueName, out var queue);
            return queue;
        }

        public IEnumerable<string> GetQueueNames() => _queues.Keys;

        public void DeleteQueue(string queueName)
        {
            _queues.TryRemove(queueName, out var queue);
            queue?.Dispose();
        }

        public void ClearQueue(string queueName)
        {
            _queues.TryGetValue(queueName, out var queue);
            queue?.Clear();
        }

        private PriorityMessageQueue CreateQueue(string queueName)
        {
            var queue = new PriorityMessageQueue();

            queue.OnStateChange += message =>
            {
                _hubContext.Clients.Group(queueName).SendAsync(Topics.QueueStateChanged, queueName, message);
            };

            return queue;
        }
    }
}
