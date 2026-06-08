using Todo.Application.DTOs.Request;
using Todo.Domain.DomainEntities;

namespace Todo.Application.Mappers;

public static class TodoMappingExtension
{
    extension(CreateTodoDto todo)
    {
        public TodoListDomain ConvertToTodoListDomain()
        {
            return new TodoListDomain()
            {
                Description = todo.description,
                Name = todo.name,
                Items = new TodoItemDomain()
                {
                    Description =  todo.Items.description,
                    Title =  todo.Items.title,  
                    Priority = todo.Items.priority,
                    DueDate = todo.Items.dueDate,   
                    ReminderDate = todo.Items.remiderDate,
                }
            };
        }
    }
}