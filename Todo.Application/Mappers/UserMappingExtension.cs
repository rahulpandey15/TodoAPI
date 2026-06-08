using Todo.Application.DTOs.Request;
using Todo.Domain.DomainEntities;

namespace Todo.Application.Mappers
{
    public static class UserMappingExtension
    {
        extension(CreateUserDto user)
        {
            public UserDomain ToUserDomain()
            {
                return new UserDomain()
                {
                    Email = user.Email,
                    FullName = user.FullName,
                    PasswordHash = user.Password
                };
            }
        }
    }
}
