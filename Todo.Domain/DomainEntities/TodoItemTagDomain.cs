using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Domain.DomainEntities
{
    public class TodoItemTagDomain
    {
        public Guid Id { get; set; }
        public Guid TodoItemId { get; set; }
        public TodoItemDomain TodoItem { get; set; }

        public Guid TagId { get; set; }
        public TagDomain Tag { get; set; }
    }
}
