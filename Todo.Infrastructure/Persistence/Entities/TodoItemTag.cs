namespace Todo.Infrastructure.Persistence.Entities
{
    public class TodoItemTag
    {
        public Guid TodoItemId { get; set; }
        public TodoItem TodoItem { get; set; }

        public Guid TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
