using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Todo.Domain.RepositoryInterface;
using Todo.Infrastructure.Persistence.Entities;

namespace Todo.Infrastructure.Repository
{
    public class GenericRepository<TDomain, TEntity> : IGenericRepository<TDomain>
        where TDomain : class
        where TEntity : class
    {
        private readonly TodoAppDbContext todoAppDbContext;
        private readonly IMapper mapper;

        public GenericRepository(TodoAppDbContext todoAppDbContext,
            IMapper mapper)
        {
            this.todoAppDbContext = todoAppDbContext;
            this.mapper = mapper;
        }

        public async Task AddAsync(TDomain domain)
        {
            var entity = mapper.Map<TEntity>(domain); // domain --> enttity

            await todoAppDbContext.Set<TEntity>().AddAsync(entity);
        }

        public async Task<int> CommitAsync()
        {
            return await todoAppDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<TDomain>> GetAllAsync()
        {
            return await todoAppDbContext.Set<TEntity>()
               .ProjectTo<TDomain>(mapper.ConfigurationProvider)
               .ToListAsync();
        }

        public async Task<TDomain?> GetByIdAsync(object id)
        {
            var entity = await todoAppDbContext.Set<TEntity>().FindAsync(id); // id shd be primary key

            return entity == null ? null : mapper.Map<TDomain>(entity); // enttity---> domain
        }
    }
}
