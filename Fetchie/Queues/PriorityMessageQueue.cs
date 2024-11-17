using Fetchie.Models;

public class PriorityMessageQueue : IDisposable
{
    private bool _disposed;

    private readonly object _lock = new();
    private readonly List<Message> _queue = new();
    public event Action<Message?>? OnStateChange;

    private Message? _currentState;

    public void Enqueue(Message message)
    {
        if (message.Expiration.HasValue && message.Expiration.Value <= DateTime.UtcNow)
        {
            return;
        }

        var index = _queue.BinarySearch(message, new MessageComparer());
        if (index < 0) index = ~index;
        
        lock (_lock)
        {
            _queue.Insert(index, message);
        }

        UpdateState();
    }

    public void RemoveExpired()
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            _queue.RemoveAll(message => message.Expiration.HasValue && message.Expiration.Value <= now);
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

    private void UpdateState()
    {
        var nextState = _queue.FirstOrDefault();

        if (_currentState != nextState)
        {
            lock (_lock)
            {
                _currentState = nextState;
            }

            OnStateChange?.Invoke(nextState);
        }
    }

    public Message? GetCurrentState() => _currentState;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                
            }

            OnStateChange = null;
            _disposed = true;
        }
    }


    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
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