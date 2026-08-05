using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Todo.Domain.DomainEntities;
using Todo.Domain.RepositoryInterface;
using Todo.Infrastructure.Persistence.Entities;

namespace Todo.Infrastructure.Repository
{
    public class TodoRepository :
        GenericRepository<TodoListDomain, TodoList>, ITodoRepository
    {
        private readonly TodoAppDbContext todoAppDbContext;

        public TodoRepository(
            TodoAppDbContext todoAppDbContext, 
            IMapper mapper) 
            : base(todoAppDbContext, mapper)
        {
            this.todoAppDbContext = todoAppDbContext;
        }

        public async Task<List<TodoListDomain>> GetTodosAsync(Guid userId)
        {
            var todoItems
                = await todoAppDbContext.TodoLists
                    .Include(x => x.TodoItems)
                    .Where(x => x.UserId == userId)
                    .ToListAsync();

            return _mapper.Map<List<TodoListDomain>>(todoItems);
        }
    }
}
