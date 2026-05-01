using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Domain.DomainEntities
{
    public class TodoItemDomain
    {
        public Guid TodoListId { get; set; }
        public TodoListDomain TodoList { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public int Priority { get; set; }
        public int Status { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? ReminderDate { get; set; }
        public DateTime? CompletedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
