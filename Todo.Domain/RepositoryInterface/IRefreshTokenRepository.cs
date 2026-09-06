using Todo.Domain.DomainEntities;

namespace Todo.Domain.RepositoryInterface
{
    /// <summary>
    /// Repository interface for RefreshToken domain entity.
    /// Provides methods for managing refresh tokens.
    /// </summary>
    public interface IRefreshTokenRepository : IGenericRepository<RefreshTokenDomain>
    {
        /// <summary>
        /// Gets a refresh token by its token hash.
        /// </summary>
        /// <param name="tokenHash">The SHA256 hash of the refresh token.</param>
        /// <returns>The refresh token domain object if found, null otherwise.</returns>
        Task<RefreshTokenDomain?> GetByRefreshTokenAsync(string tokenHash);

        /// <summary>
        /// Gets all active (non-expired, non-revoked) refresh tokens for a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A collection of active refresh tokens for the user.</returns>
        Task<IEnumerable<RefreshTokenDomain>> GetActiveTokensByUserIdAsync(Guid userId);

        /// <summary>
        /// Gets a refresh token by ID with related entities.
        /// </summary>
        /// <param name="id">The refresh token ID.</param>
        /// <returns>The refresh token domain object if found, null otherwise.</returns>
        Task<RefreshTokenDomain?> GetByIdWithDetailsAsync(Guid id);

        /// <summary>
        /// Revokes all active refresh tokens for a user (e.g., during logout).
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="revokedByIp">The IP address from which revocation occurred (optional).</param>
        /// <returns>The number of tokens revoked.</returns>
        Task<int> RevokeAllUserTokensAsync(Guid userId, string? revokedByIp = null);

        /// <summary>
        /// Deletes expired refresh tokens from the database (cleanup operation).
        /// </summary>
        /// <returns>The number of tokens deleted.</returns>
        Task<int> DeleteExpiredTokensAsync();

        /// <summary>
        /// Gets a refresh token by ID for revoked token reuse detection.
        /// </summary>
        /// <param name="id">The refresh token ID.</param>
        /// <returns>The refresh token domain object if found, null otherwise.</returns>
        Task<RefreshTokenDomain?> GetRevokedTokenAsync(Guid id);
    }
}
