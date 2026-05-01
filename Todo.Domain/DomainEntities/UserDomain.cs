using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Domain.DomainEntities
{
    public class UserDomain
    {
        public string FullName { get; set; }
        public string Email { get; set; }

        public string PasswordHash { get; set; }
    }
}
