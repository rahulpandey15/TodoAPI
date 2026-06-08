using AutoMapper;
using Todo.Application.Contracts;
using Todo.Application.DTOs.Request;
using Todo.Domain.DomainEntities;
using Todo.Domain.RepositoryInterface;

namespace Todo.Application.Implementation;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;
    private readonly IMapper _mapper;

    public TodoService(
        ITodoRepository todoRepository,
        IMapper mapper)
    {
        _todoRepository = todoRepository;
        _mapper = mapper;
    }

    public async Task<bool> CreateTodoAsync(
        CreateTodoDto todos)
    {
        var todo = _mapper.Map<TodoListDomain>(todos);

        await _todoRepository.AddAsync(todo);

        int rowsInserted = await _todoRepository.CommitAsync();

        return rowsInserted > 0;
    }
}
