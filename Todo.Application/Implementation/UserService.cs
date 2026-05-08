using AutoMapper;
using Todo.Application.Contracts;
using Todo.Application.DTOs.Request;
using Todo.Domain.DomainEntities;
using Todo.Domain.RepositoryInterface;

namespace Todo.Application.Implementation
{
    public class UserService(
              IUserRepository userRepository
            , IMapper mapper) : IUserService
    {

        public async Task<bool> CreateUserAsync(CreateUserDto userDto)
        {
            // dto into domain
            // user password into hashed password

            var userDomain = mapper.Map<UserDomain>(userDto);

            userDomain.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDomain.PasswordHash);
            
            await userRepository.AddAsync(userDomain);

            var response = await userRepository.CommitAsync();

            return response > 0;

        }
    }
}
