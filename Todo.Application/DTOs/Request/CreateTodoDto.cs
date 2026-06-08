namespace Todo.Application.DTOs.Request;

public record CreateTodoDto(
    string name,
    string description,
    CreateTodoItemsDto Items);
