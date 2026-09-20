
using Todo.Application.Common;
using Todo.Application.DTOs.Request;
using Todo.Application.DTOs.Response;

namespace Todo.Application.Contracts
{
    public interface ITodoService
    {
        Task<Result> CreateTodoAsync(CreateTodoDto todos);

        Task<Result<List<TodoResponseDto>>> GetItems();
    }
}
