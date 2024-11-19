using Dynq;
using Fetchie.Host.Dynq.Messages;
using Fetchie.Host.Models;

public class MessageQueue
{
    private readonly object _lock = new();
    private readonly List<Message> _queue = [];

    private string _queueName;
    private IDynqService _dynqService;

    public MessageQueue(string queueName, IDynqService dynqService)
    {
        _queueName = queueName;
        _dynqService = dynqService;
    }

    private Message? _head;

    public void Enqueue(Message message, bool replace = false)
    {
        if (message.ExpirationUtc.HasValue && message.ExpirationUtc.Value <= DateTime.UtcNow)
        {
            return;
        }

        lock (_lock)
        {
            if (replace && _queue.Count != 0 && _queue[0].Severity == message.Severity)
            {
                _queue.RemoveAt(0);
            }

            var index = _queue.BinarySearch(message, new MessageComparer());
            if (index < 0) index = ~index;

            _queue.Insert(index, message);
        }

        UpdateState();
    }


    public void RemoveExpired()
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            _queue.RemoveAll(message => message.ExpirationUtc.HasValue && message.ExpirationUtc.Value <= now);
        }

        UpdateState();
    }

    public void Remove(Guid messageId)
    {
        lock (_lock)
        {
            _queue.RemoveAll(message => message.MessageId == messageId);
        }

        UpdateState();
    }

    public IEnumerable<Message> GetMessages() => _queue;

    public void Clear()
    {
        lock (_lock)
        {
            _queue.Clear();
        }

        UpdateState();
    }

    private async void UpdateState()
    {
        var nextHead = _queue.FirstOrDefault();

        if (_head != nextHead)
        {
            lock (_lock)
            {
                _head = nextHead;
            }

            await _dynqService.BroadcastAsync(new QueueHeadChangedMessage
            {
                Queue = _queueName,
                Head = nextHead
            });
        }
    }

    public Message? Head { get => _head; }
}


public class MessageComparer : IComparer<Message>
{
    public int Compare(Message? x, Message? y)
    {
        if (x == null || y == null)
        {
            return 0;
        }

        var severityComparison = y.Severity.CompareTo(x.Severity);
        if (severityComparison != 0)
        {
            return severityComparison;
        }

        return x.CreatedAt.CompareTo(y.CreatedAt);
    }
}