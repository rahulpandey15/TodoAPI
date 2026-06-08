using Todo.Application.Mappers;
using Todo.Application.Contracts;
using Todo.Application.DTOs.Request;
using Todo.Domain.RepositoryInterface;

namespace Todo.Application.Implementation;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;

    public TodoService(
        ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<bool> CreateTodoAsync(
        CreateTodoDto todos)
    {
        var todo
            = todos.ConvertToTodoListDomain();

        await _todoRepository.AddAsync(todo);

        int rowsInserted = await _todoRepository.CommitAsync();

        return rowsInserted > 0;
    }
}