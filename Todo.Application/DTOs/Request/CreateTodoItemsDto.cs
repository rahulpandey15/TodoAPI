namespace Todo.Application.DTOs.Request;

public record CreateTodoItemsDto(
    string title,
    string description,
    string priority,
    string status,
    DateTime dueDate,
    DateTime remiderDate
    );
