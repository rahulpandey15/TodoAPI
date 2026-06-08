using Todo.Application.Contracts;
using Todo.Application.DTOs.Request;
using Todo.Application.Mappers;
using Todo.Domain.RepositoryInterface;

namespace Todo.Application.Implementation
{
    public class UserService(
              IUserRepository userRepository,
              IPasswordHasher _passwordHasher) : IUserService
    {

        public async Task<bool> CreateUserAsync(CreateUserDto userDto)
        {
            // dto into domain
            // user password into hashed password

            var userDomain =  userDto.ToUserDomain();   

            userDomain.PasswordHash = _passwordHasher.Hash(userDto.Password);    
            
            await userRepository.AddAsync(userDomain);

            var response = await userRepository.CommitAsync();

            return response > 0;

        }
    }
}
