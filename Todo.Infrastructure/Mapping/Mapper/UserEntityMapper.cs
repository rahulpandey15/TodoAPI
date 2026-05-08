using Todo.Domain.DomainEntities;
using Todo.Infrastructure.Mappers;
using Todo.Infrastructure.Persistence.Entities;

namespace Todo.Infrastructure.Mapping.Mapper
{
    public class UserEntityMapper : IMapper<UserDomain, User>
    {
        public User Map(UserDomain source)
        {
            return new User()
            {
                Email = source.Email,
                FullName = source.FullName,
                PasswordHash = source.PasswordHash
            };
        }
    }
}
