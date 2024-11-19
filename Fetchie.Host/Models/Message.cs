namespace Fetchie.Host.Models
{
    public class Message
    {
        public required Guid MessageId { get; init; }

        public required string Title { get; set; }

        public required string Body { get; set; }

        public required MessageSeverity Severity { get; set; }

        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public required DateTime? ExpirationUtc { get; set; }

        public required string Queue { get; set; }
    }

    public enum MessageSeverity
    {
        None,
        Information,
        Warning,
        Error,
        Critical
    }
}
