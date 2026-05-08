using Todo.Domain.RepositoryInterfaces;
using Todo.Infrastructure.Mapping;
using Todo.Infrastructure.Persistence.Entities;

namespace Todo.Infrastructure.Repositories
{
    public class GenericRepository<TDomain, TEntity>(
        TodoAppDbContext todoAppDbContext,
        IMappingService mappingService)
        : IGenericRepository<TDomain>
        where TDomain : class
        where TEntity : class

    {
        public async Task AddAsync(TDomain domainEntity)
        {
            var enttity = mappingService.Map<TDomain,TEntity>(domainEntity);
            await todoAppDbContext.Set<TEntity>().AddAsync(enttity);
        }

        public async Task<int> CommitAsync()
        {
            return await todoAppDbContext.SaveChangesAsync();
        }

        public Task<TDomain> GetAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
