using System.Security.Cryptography;
using System.Text;

namespace Todo.Infrastructure.Persistence.Entities
{
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
    }
}
