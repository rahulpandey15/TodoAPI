using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Todo.Domain.DomainEntities;
using Todo.Domain.RepositoryInterface;
using Todo.Infrastructure.Persistence.Entities;

namespace Todo.Infrastructure.Repository
{
    public class UserRepository : GenericRepository<UserDomain, User>, IUserRepository
    {
        public UserRepository(
            TodoAppDbContext todoAppDbContext, 
            IMapper mapper) : base(todoAppDbContext, mapper)
        {

        }
    }
}
