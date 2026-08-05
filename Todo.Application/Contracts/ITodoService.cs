
using Todo.Application.DTOs.Request;
using Todo.Application.DTOs.Response;

namespace Todo.Application.Contracts
{
    public interface ITodoService
    {
        Task<bool> CreateTodoAsync(CreateTodoDto todos);

        Task<IEnumerable<TodoResponseDto>> GetItems();
    }
}
