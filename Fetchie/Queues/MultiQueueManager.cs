﻿using Fetchie.Models;
using Fetchie.SignalR;
using Fetchie.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Fetchie.Queues
{
    public class MultiQueueManager
    {
        private readonly IHubContext<LogHub> _hubContext;

        public MultiQueueManager(IHubContext<LogHub> hubContext)
        {
            _hubContext = hubContext;
        }

        private readonly ConcurrentDictionary<string, PriorityMessageQueue> _queues = new();

        public void Enqueue(Message message)
        {
            var queue = _queues.GetOrAdd(message.Queue, _ => CreateQueue(message.Queue));

            queue.Enqueue(message);
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

        public void PurgeQueue(string queueName)
        {
            _queues.TryRemove(queueName, out _);
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