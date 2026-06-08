namespace Todo.Domain.DomainEntities;
public class TodoListDomain
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public TodoListDomain Items { get; set; } = new();
    public Guid UserId { get; set; }
    public UserDomain User { get; set; } = new();
}
