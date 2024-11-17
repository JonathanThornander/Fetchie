﻿namespace Fetchie.Models
{
    public class Message
    {
        public required Guid MessageId { get; init; }
        public required string Title { get; set; }
        public required string Body { get; set; }
        public required Severity Severity { get; set; }
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required DateTime? Expiration { get; set; }
        public required string Queue { get; set; }
    }

    public enum Severity
    {
        Low,
        Medium,
        High,
        Critical
    }
}
