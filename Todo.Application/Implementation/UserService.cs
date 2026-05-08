using Todo.Application.DTO;
using Todo.Application.Interface;
using Todo.Domain.DomainEntities;
using Todo.Domain.RepositoryInterfaces;

namespace Todo.Application.Implementation
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        public async Task<bool> AddUser(UserDto userDto)
        {
            UserDomain userDomain
                = new()
                {
                    Email = userDto.Email,
                    FullName = userDto.FullName,
                    PasswordHash = userDto.Password
                };
            
            var x = userRepository.AddAsync(userDomain);

            return await Task.FromResult(true);
        }
    }
}
