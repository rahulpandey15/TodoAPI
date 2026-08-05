namespace Todo.Application.DTOs.Request;

public record CreateTodoDto(
    string name,
    string description,
    List<CreateTodoItemsDto> Items);
