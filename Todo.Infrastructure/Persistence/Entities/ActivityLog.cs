namespace Todo.Infrastructure.Persistence.Entities
{
    public class ActivityLog : BaseEntity
    {
        public string EntityName { get; set; }
        public Guid EntityId { get; set; }

        public string Action { get; set; }
        public string PerformedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string OldValues { get; set; }
        public string NewValues { get; set; }
    }
}
