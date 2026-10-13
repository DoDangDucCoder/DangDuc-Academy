using NodaTime;

namespace HDNXUdemyData.Entities
{
    public class AuditLogEntities : BaseEntities
    {
        public long? UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public LocalDateTime CreatedAt { get; set; }
    }
}
