namespace Todo.Domain.DomainEntities
{

    public class RefreshTokenDomain
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? CreatedByIp { get; set; }
        public string? RevokedByIp { get; set; }
        public Guid? ReplacedByTokenId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

       
        public bool IsActive() => !IsExpired() && !IsRevoked();
        public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
        public bool IsRevoked() => RevokedAt.HasValue && RevokedAt.Value <= DateTime.UtcNow;
    }
}
