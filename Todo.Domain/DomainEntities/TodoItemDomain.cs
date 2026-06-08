using Todo.Domain.Enums;

namespace Todo.Domain.DomainEntities;

public class TodoItemDomain
{
    public Guid TodoListId { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public TodoPriority Priority { get; set; } = TodoPriority.Normal;
    public TodoStatus Status { get; set; } = TodoStatus.New;
    public DateTime? DueDate { get; set; }
    public DateTime? ReminderDate { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    public bool IsDeleted { get; set; }


    public void  MarkAsCompleted()
    {
        Status = TodoStatus.Completed;
    }

    public void MarkAsInProgress()
    {
        Status = TodoStatus.InProgress;
    }
}
