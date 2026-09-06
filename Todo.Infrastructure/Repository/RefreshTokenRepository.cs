using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Todo.Domain.DomainEntities;
using Todo.Domain.RepositoryInterface;
using Todo.Infrastructure.Persistence.Entities;

namespace Todo.Infrastructure.Repository
{

    public class RefreshTokenRepository :
         GenericRepository<RefreshTokenDomain, RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(
            TodoAppDbContext todoAppDbContext,
            IMapper mapper) : base(todoAppDbContext, mapper)
        {
        }

        public async Task<RefreshTokenDomain?> GetByRefreshTokenAsync(string refreshToken)
        {
            var token = await _todoAppDbContext.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            return token == null ? null : _mapper.Map<RefreshTokenDomain>(token);
        }


        public async Task<IEnumerable<RefreshTokenDomain>> GetActiveTokensByUserIdAsync(Guid userId)
        {
            var now = DateTime.UtcNow;

            var tokens = await _todoAppDbContext.RefreshTokens
                .Where(x => x.UserId == userId
                    && x.ExpiresAt > now
                    && x.RevokedAt == null)
                .ToListAsync();

            return tokens.Select(x => _mapper.Map<RefreshTokenDomain>(x));
        }


        public async Task<RefreshTokenDomain?> GetByIdWithDetailsAsync(Guid id)
        {
            var token = await _todoAppDbContext.RefreshTokens
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.ReplacedByToken)
                .FirstOrDefaultAsync(x => x.Id == id);

            return token == null ? null : _mapper.Map<RefreshTokenDomain>(token);
        }

        public async Task<int> RevokeAllUserTokensAsync(Guid userId, string? revokedByIp = null)
        {
            var now = DateTime.UtcNow;

            var tokens = await _todoAppDbContext.RefreshTokens
                .Where(x => x.UserId == userId
                    && x.ExpiresAt > now
                    && x.RevokedAt == null)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = revokedByIp;
            }

            return await _todoAppDbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteExpiredTokensAsync()
        {
            var now = DateTime.UtcNow;

            var expiredTokens = await _todoAppDbContext.RefreshTokens
                .Where(x => x.ExpiresAt < now)
                .ToListAsync();

            if (expiredTokens.Count == 0)
                return 0;

            _todoAppDbContext.RefreshTokens.RemoveRange(expiredTokens);
            return await _todoAppDbContext.SaveChangesAsync();
        }

        public async Task<RefreshTokenDomain?> GetRevokedTokenAsync(Guid id)
        {
            var token = await _todoAppDbContext.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.RevokedAt != null);

            return token == null ? null : _mapper.Map<RefreshTokenDomain>(token);
        }

        public async Task<RefreshTokenDomain> GetRefreshTokenById(Guid id)
        {
            var refreshToken = await _todoAppDbContext.RefreshTokens
                         .AsNoTracking()
                         .FirstOrDefaultAsync(rt => rt.Id == id);

            return refreshToken == null ? null : _mapper.Map<RefreshTokenDomain>(refreshToken);
        }
    }
}
