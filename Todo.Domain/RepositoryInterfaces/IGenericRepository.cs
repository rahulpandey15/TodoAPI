using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Domain.RepositoryInterfaces
{
    public interface IGenericRepository<TDomain> where TDomain : class
    {

        Task AddAsync(TDomain domainEntity); // add new entity

        Task<TDomain> GetAsync(int id);  // giving data based on Id

        Task<int> CommitAsync();
    } 
}
