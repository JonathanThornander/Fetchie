using Fetchie.Models;

public class PriorityMessageQueue : IDisposable
{
    private bool _disposed;

    private readonly object _lock = new();
    private readonly List<Message> _queue = [];
    public event Action<Message?>? OnStateChange;

    private Message? _currentState;

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