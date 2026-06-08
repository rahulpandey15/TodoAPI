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
        protected readonly TodoAppDbContext _todoAppDbContext;
        protected readonly IMapper _mapper;

        public GenericRepository(TodoAppDbContext todoAppDbContext,
            IMapper mapper)
        {
            _todoAppDbContext = todoAppDbContext;
            _mapper = mapper;
        }

        public async Task AddAsync(TDomain domain)
        {
            var entity = _mapper.Map<TEntity>(domain); // domain --> enttity

            await _todoAppDbContext.Set<TEntity>().AddAsync(entity);
        }

        public async Task<int> CommitAsync()
        {
            return await _todoAppDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<TDomain>> GetAllAsync()
        {
            return await _todoAppDbContext.Set<TEntity>()
                .ProjectTo<TDomain>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<TDomain?> GetByIdAsync(object id)
        {
            var entity = await _todoAppDbContext.Set<TEntity>().FindAsync(id); // id shd be primary key

            return entity == null ? null : _mapper.Map<TDomain>(entity); // enttity---> domain
        }
    }
}