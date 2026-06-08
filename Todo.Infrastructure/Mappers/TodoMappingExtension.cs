using AutoMapper;
using Todo.Domain.DomainEntities;
using Todo.Infrastructure.Persistence.Entities;

namespace Todo.Infrastructure.Mappers;

public class TodoMappingExtension : Profile
{
    public TodoMappingExtension()
    {
        CreateMap<TodoItemDomain, TodoItem>()
            .ForMember(dest => dest.Priority, opt 
                => opt.MapFrom(src => (int)src.Priority))
            .ForMember(dest => dest.Status, opt 
                => opt.MapFrom(src => (int)src.Status));

        CreateMap<TodoListDomain, TodoList>()
            .ForMember(dest => dest.UserId, opt 
                => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.TodoItems,
                opt => opt.MapFrom(src 
                    => src.Items == null ? null : new[] { src.Items }));
        
  
        
    }
}