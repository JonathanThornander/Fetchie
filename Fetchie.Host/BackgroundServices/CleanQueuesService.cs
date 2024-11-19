using Fetchie.Host.Queues;

namespace Fetchie.Host.BackgroundServices
{
    public class MultiQueueCleanupService : BackgroundService
    {
        private readonly MessageQueueManager _queueManager;
        private readonly Dictionary<string, Task> _scheduledTasks = new();

        public MultiQueueCleanupService(MessageQueueManager queueManager)
        {
            _queueManager = queueManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var queueName in _queueManager.GetQueueNames())
                {
                    if (_queueManager.GetQueue(queueName) is { } queue)
                    {
                        ScheduleMessageExpiration(queue, queueName, stoppingToken);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private void ScheduleMessageExpiration(MessageQueue queue, string queueName, CancellationToken stoppingToken)
        {
            var expiringMessages = queue.GetMessages()
                .Where(msg => msg.ExpirationUtc.HasValue &&
                              msg.ExpirationUtc.Value > DateTime.UtcNow &&
                              msg.ExpirationUtc.Value <= DateTime.UtcNow.AddSeconds(10))
                .ToList();

            foreach (var message in expiringMessages)
            {
                var expirationTime = message.ExpirationUtc!.Value;

                var taskKey = $"{queueName}_{message.CreatedAt}";

                if (_scheduledTasks.ContainsKey(taskKey))
                {
                    continue;
                }

                var delay = expirationTime - DateTime.UtcNow;
                var task = Task.Delay(delay, stoppingToken).ContinueWith(_ =>
                {
                    queue.RemoveExpired(); // Remove expired messages
                    _scheduledTasks.Remove(taskKey); // Cleanup task dictionary
                }, stoppingToken);

                _scheduledTasks[taskKey] = task; // Track the scheduled task
            }
        }
    }
}
