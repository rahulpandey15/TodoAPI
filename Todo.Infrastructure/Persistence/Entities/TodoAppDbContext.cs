using Microsoft.EntityFrameworkCore;
using Todo.Application.Contracts;

namespace Todo.Infrastructure.Persistence.Entities
{
    public class TodoAppDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUser;

        public TodoAppDbContext(
            DbContextOptions<TodoAppDbContext> options,
            ICurrentUserService currentUser) : base(options)
        {
            _currentUser = currentUser;
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ApplyAuditConfig();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            ApplyAuditConfig();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }


        private void ApplyAuditConfig()
        {
            var entries = ChangeTracker
                .Entries<BaseAuditableEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy
                        = _currentUser.GetCurrentUserId() ?? "system";
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy
                        = _currentUser.GetCurrentUserId() ?? "system";
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodoAppDbContext).Assembly);

            modelBuilder.Entity<TodoList>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TodoItem>()
                .HasOne(x => x.TodoList)
                .WithMany(x => x.TodoItems)
                .HasForeignKey(x => x.TodoListId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(x => x.TodoItem)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.TodoItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TodoItemTag>()
                .HasOne(x => x.TodoItem)
                .WithMany(x => x.TodoItemTags)
                .HasForeignKey(x => x.TodoItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TodoItemTag>()
                .HasOne(x => x.Tag)
                .WithMany(x => x.TodoItemTags)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        }


        public DbSet<User> Users { get; set; }
        public DbSet<TodoList> TodoLists { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TodoItemTag> TodoItemTags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
