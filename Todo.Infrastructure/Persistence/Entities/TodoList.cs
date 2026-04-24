namespace Todo.Infrastructure.Persistence.Entities
{
    public class TodoList : BaseAuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public ICollection<TodoItem> TodoItems { get; set; }
    }
}
