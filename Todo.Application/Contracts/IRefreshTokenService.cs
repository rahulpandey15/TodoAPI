using Todo.Domain.DomainEntities;

namespace Todo.Application.Contracts
{
    public interface IRefreshTokenService
    {
        Task<RefreshTokenDomain> GetRefreshTokenById(Guid id);
    }
}
