namespace Todo.Domain.DomainEntities;
public class TodoListDomain
{
    public string Name { get; set; }
    public string Description { get; set; }
    public TodoItemDomain Items { get; set; }
    public Guid UserId { get; set; }
    public UserDomain User { get; set; }
}
