using AutoMapper;
using Todo.Domain.DomainEntities;
using Todo.Domain.RepositoryInterface;
using Todo.Infrastructure.Persistence.Entities;

namespace Todo.Infrastructure.Repository
{
    public class TodoRepository :
        GenericRepository<TodoListDomain, TodoList>, ITodoRepository
    {
        public TodoRepository(TodoAppDbContext todoAppDbContext, IMapper mapper) 
            : base(todoAppDbContext, mapper)
        {
        }
    }
}
