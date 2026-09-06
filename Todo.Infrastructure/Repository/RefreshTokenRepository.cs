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

        /// <summary>
        /// Gets a refresh token by its token hash.
        /// </summary>
        public async Task<RefreshTokenDomain?> GetByRefreshTokenAsync(string refreshToken)
        {
            var token = await _todoAppDbContext.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            return token == null ? null : _mapper.Map<RefreshTokenDomain>(token);
        }

        /// <summary>
        /// Gets all active (non-expired, non-revoked) refresh tokens for a user.
        /// </summary>
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

        /// <summary>
        /// Gets a refresh token by ID with related entities (if any).
        /// </summary>
        public async Task<RefreshTokenDomain?> GetByIdWithDetailsAsync(Guid id)
        {
            var token = await _todoAppDbContext.RefreshTokens
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.ReplacedByToken)
                .FirstOrDefaultAsync(x => x.Id == id);

            return token == null ? null : _mapper.Map<RefreshTokenDomain>(token);
        }

        /// <summary>
        /// Revokes all active refresh tokens for a user.
        /// Used during logout or security events.
        /// </summary>
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

        /// <summary>
        /// Deletes expired refresh tokens from the database.
        /// This is a maintenance operation to remove old tokens.
        /// </summary>
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

        /// <summary>
        /// Gets a revoked refresh token by ID for reuse detection.
        /// Used to detect if a previously revoked token is being reused (token theft).
        /// </summary>
        public async Task<RefreshTokenDomain?> GetRevokedTokenAsync(Guid id)
        {
            var token = await _todoAppDbContext.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.RevokedAt != null);

            return token == null ? null : _mapper.Map<RefreshTokenDomain>(token);
        }
    }
}
