namespace Todo.Application.DTOs.Request;

public record CreateTodoDto(
    string name,
    string description,
    Guid userId,
    CreateTodoItemsDto Items);
