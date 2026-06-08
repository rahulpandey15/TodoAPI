
using Todo.Application.DTOs.Request;

namespace Todo.Application.Contracts
{
    public interface ITodoService
    {
        Task<bool> CreateTodoAsync(CreateTodoDto todos);
    }
}
