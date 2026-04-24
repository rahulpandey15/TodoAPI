namespace Todo.Infrastructure.Persistence.Entities
{
    public class Tag : BaseAuditableEntity
    {
        public string Name { get; set; }
        public string ColorCode { get; set; }

        public ICollection<TodoItemTag> TodoItemTags { get; set; }
    }
}
