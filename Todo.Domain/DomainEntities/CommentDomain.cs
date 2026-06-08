namespace Todo.Domain.DomainEntities
{
    public class CommentDomain
    {
        public Guid TodoItemId { get; set; }
        public TodoItemDomain TodoItem { get; set; }
        public Guid UserId { get; set; }
        public UserDomain User { get; set; }
        public string Message { get; set; }
    }
}
