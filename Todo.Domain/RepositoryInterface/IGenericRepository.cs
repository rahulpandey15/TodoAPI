using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Domain.RepositoryInterface
{
    public interface IGenericRepository<TDomain> where TDomain : class
    {
        Task<TDomain> GetByIdAsync(object id);

        Task<IEnumerable<TDomain>> GetAllAsync();

        Task AddAsync(TDomain domain);

        Task<int> CommitAsync();


    }
}
