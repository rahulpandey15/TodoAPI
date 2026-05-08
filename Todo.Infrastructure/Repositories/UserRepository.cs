using Todo.Domain.DomainEntities;
using Todo.Domain.RepositoryInterfaces;
using Todo.Infrastructure.Mapping;
using Todo.Infrastructure.Persistence.Entities;

namespace Todo.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<UserDomain, User>, IUserRepository
    {
        public UserRepository(
            TodoAppDbContext todoAppDbContext,
            IMappingService mappingService) 
            : base(todoAppDbContext, mappingService)
        {
        }
    }
}
