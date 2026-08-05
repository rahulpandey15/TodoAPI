using Todo.Domain.DomainEntities;

namespace Todo.Domain.RepositoryInterface;

public interface ITodoRepository : IGenericRepository<TodoListDomain>
{
    Task<List<TodoListDomain>> GetTodosAsync(Guid userId);
}
