namespace Todo.Application.DTOs.Response
{
    public record TodoResponseDto(
        string Name,
        string Description,
        List<TodoMetadata>? Metadata
    );

    public record TodoMetadata(
        string Title,
        string Description,
        string Priority,
        string Status,
        DateTime? DueDate
    );
}

