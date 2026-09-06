using System.Security.Cryptography;
using System.Text;

namespace Todo.Infrastructure.Persistence.Entities
{
    /// <summary>
    /// Represents a refresh token for user authentication.
    /// Refresh tokens are single-use and rotated on each use.
    /// NOTE: This entity only contains database-level concerns (properties and navigation).
    /// Business logic for token generation/hashing is in RefreshTokenUtility.
    /// </summary>
    public class RefreshToken : BaseAuditableEntity
    {

        public Guid UserId { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? CreatedByIp { get; set; }
        public string? RevokedByIp { get; set; }
        public Guid? ReplacedByTokenId { get; set; }
        public RefreshToken? ReplacedByToken { get; set; }
        public ICollection<RefreshToken> ReplacementTokens { get; set; } = [];

        /// <summary>
        /// Determines if the refresh token is active (not expired and not revoked).
        /// </summary>
        public bool IsActive() => !IsExpired() && !IsRevoked();

        /// <summary>
        /// Determines if the refresh token has expired.
        /// </summary>
        public bool IsExpired() => DateTime.UtcNow > ExpiresAt;

        /// <summary>
        /// Determines if the refresh token has been revoked.
        /// </summary>
        public bool IsRevoked() => RevokedAt.HasValue && RevokedAt.Value <= DateTime.UtcNow;

        /// <summary>
        /// Revokes this refresh token by setting the RevokedAt and RevokedByIp properties.
        /// </summary>
        public void Revoke(string? revokedByIp = null)
        {
            RevokedAt = DateTime.UtcNow;
            RevokedByIp = revokedByIp;
        }
    }
}
