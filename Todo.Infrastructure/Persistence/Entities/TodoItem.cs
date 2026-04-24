namespace Todo.Infrastructure.Persistence.Entities
{
    public class TodoItem : BaseAuditableEntity
    {
        public Guid TodoListId { get; set; }
        public TodoList TodoList { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public int Priority { get; set; }
        public int Status { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? ReminderDate { get; set; }
        public DateTime? CompletedAt { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<TodoItemTag> TodoItemTags { get; set; } // Navigation Properties
        public ICollection<Comment> Comments { get; set; }
    }
}
