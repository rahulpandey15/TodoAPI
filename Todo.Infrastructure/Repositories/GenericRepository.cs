using System;
using System.Collections.Generic;
using System.Text;
using Todo.Domain.RepositoryInterfaces;
using Todo.Infrastructure.Persistence.Entities;

namespace Todo.Infrastructure.Repositories
{
    public class GenericRepository<TDomain, TEntity> : IGenericRepository<TDomain>
        where TDomain : class
        where TEntity : class
           
    {
        private readonly TodoAppDbContext todoAppDbContext;
        private readonly Func<TDomain, TEntity> _mapToEntity;

        public GenericRepository(TodoAppDbContext todoAppDbContext, Func<TDomain, TEntity> mapToEntity)
        {
            this.todoAppDbContext = todoAppDbContext;
            _mapToEntity = mapToEntity;
        }

        public async Task AddAsync(TDomain domainEntity)
        {

            var enttity = _mapToEntity(domainEntity);

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
