using Todo.Application.DTOs.Request;

namespace Todo.Application.Contracts;

public interface IUserService
{
    Task<bool> CreateUserAsync(CreateUserDto userDto);
}
