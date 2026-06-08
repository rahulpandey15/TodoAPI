using AutoMapper;
using Todo.Application.DTOs.Request;
using Todo.Domain.DomainEntities;

namespace Todo.Application.Mappers
{
    public class TodoServiceMappingExtension : Profile
    {
        public TodoServiceMappingExtension()
        {
            CreateMap<CreateTodoDto, TodoListDomain>();
            CreateMap<CreateTodoItemsDto, TodoItemDomain>();
        }
    }
}
