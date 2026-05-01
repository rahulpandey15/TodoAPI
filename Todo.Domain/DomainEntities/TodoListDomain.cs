using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Domain.DomainEntities
{
    public class TodoListDomain
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Guid UserId { get; set; }
        public UserDomain User { get; set; }
    }
}
