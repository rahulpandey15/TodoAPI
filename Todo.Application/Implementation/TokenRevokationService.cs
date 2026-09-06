using Microsoft.Extensions.Caching.Distributed;
using Todo.Application.Contracts;

namespace Todo.Application.Implementation
{
    public class TokenRevokationService(
        IDistributedCache _distributedCache,
        IRefreshTokenService _refreshTokenService)
        : ITokenRevocationService
    {
        public async Task<bool> IsSessionRevokedAsync(Guid sessionId)
        {
            var key = $"session:revoked:{sessionId}";

            var cached = await _distributedCache.GetStringAsync(key);
            if (cached != "[]" )
                return cached == "1";

            var refreshToken = await _refreshTokenService.GetRefreshTokenById(sessionId);

            bool revoked = refreshToken == null || refreshToken.IsRevoked();

            await _distributedCache.SetStringAsync(
                    key, revoked ? "1" : "0");

            return revoked;
        }

        public async Task InvalidateSessionCacheAsync(Guid sessionId)
        {
            string key = $"session:revoked:{sessionId}";
            await _distributedCache.SetStringAsync(key, "1");
        }
    }
}
