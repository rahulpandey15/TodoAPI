using Todo.Application.Mappers;
using Todo.Application.Contracts;
using Todo.Application.DTOs.Request;
using Todo.Domain.RepositoryInterface;
using Todo.Application.DTOs.Response;

namespace Todo.Application.Implementation;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;
    private readonly ICurrentUserService _currentUserService;

    public TodoService(
        ITodoRepository todoRepository,
        ICurrentUserService currentUserService)
    {
        _todoRepository = todoRepository;
        _currentUserService = currentUserService;
    }

    public async Task<bool> CreateTodoAsync(
        CreateTodoDto todos)
    {
        var todo
            = todos.ConvertToTodoListDomain();

        todo.UserId = Guid.Parse(_currentUserService.GetCurrentUserId());

        await _todoRepository.AddAsync(todo);

        int rowsInserted = await _todoRepository.CommitAsync();

        return rowsInserted > 0;
    }

    public async Task<IEnumerable<TodoResponseDto>> GetItems()
    {
        var todoItemsDomain
               = await _todoRepository.GetTodosAsync(Guid.Parse(_currentUserService.GetCurrentUserId()));

        return todoItemsDomain.ToResponseDtos();
    }
}