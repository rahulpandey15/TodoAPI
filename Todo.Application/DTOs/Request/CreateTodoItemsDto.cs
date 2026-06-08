using Todo.Domain.Enums;

namespace Todo.Application.DTOs.Request;

public record CreateTodoItemsDto(
    string title,
    string description,
    TodoPriority priority,
    DateTime dueDate,
    DateTime remiderDate
    );
