namespace Todo.Infrastructure.Persistence.Entities
{
    public class User : BaseAuditableEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }

        public string PasswordHash { get; set;  }
    }
}
