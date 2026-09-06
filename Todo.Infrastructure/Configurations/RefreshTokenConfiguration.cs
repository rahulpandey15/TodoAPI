using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Infrastructure.Persistence.Entities;

namespace Todo.Infrastructure.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for RefreshToken.
    /// Defines the database schema, relationships, and constraints.
    /// </summary>
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            // Primary key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(255); // SHA256 in base64 is 44 characters, but allow for future changes

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.RevokedAt)
                .IsRequired(false);

            builder.Property(x => x.CreatedByIp)
                .IsRequired(false)
                .HasMaxLength(45); // IPv6 max length

            builder.Property(x => x.RevokedByIp)
                .IsRequired(false)
                .HasMaxLength(45);

            builder.Property(x => x.ReplacedByTokenId)
                .IsRequired(false);

            // Audit properties
            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(x => x.UpdatedAt)
                .IsRequired(false);

            builder.Property(x => x.CreatedBy)
                .IsRequired(false)
                .HasMaxLength(450);

            builder.Property(x => x.UpdatedBy)
                .IsRequired(false)
                .HasMaxLength(450);

            // Relationships
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ReplacedByToken)
                .WithMany(x => x.ReplacementTokens)
                .HasForeignKey(x => x.ReplacedByTokenId)
                .OnDelete(DeleteBehavior.NoAction);

            // Indexes for efficient lookups
            builder.HasIndex(x => x.UserId)
                .HasName("IX_RefreshToken_UserId");

            builder.HasIndex(x => x.Token)
                .HasName("IX_RefreshToken_TokenHash")
                .IsUnique(false); // Not unique because we might query by hash

            builder.HasIndex(x => x.ExpiresAt)
                .HasName("IX_RefreshToken_ExpiresAt");

            builder.HasIndex(x => new { x.UserId, x.RevokedAt })
                .HasName("IX_RefreshToken_UserId_RevokedAt");

            // Table name
            builder.ToTable("RefreshTokens");
        }
    }
}
