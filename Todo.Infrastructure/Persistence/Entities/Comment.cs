namespace Todo.Infrastructure.Persistence.Entities
{
    public class Comment : BaseAuditableEntity
    {
        public Guid TodoItemId { get; set; }
        public TodoItem TodoItem { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public string Message { get; set; }
    }
}
