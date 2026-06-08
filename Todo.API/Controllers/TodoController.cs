using Microsoft.AspNetCore.Mvc;
using Todo.Application.Contracts;
using Todo.Application.DTOs.Request;
using Todo.Application.DTOs.Response;
using Microsoft.AspNetCore.Authorization;

namespace Todo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            this._todoService = todoService;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<TodoResponseDto> todoList = [];

            todoList.Add(new TodoResponseDto(Name: "Start Learning Langchain", IsCompleted: true));
            todoList.Add(new TodoResponseDto(Name: "Start Learning VectorDb", IsCompleted: true));
            return Ok(todoList);
        }


        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody] CreateTodoDto todo)
        {
            var created = await _todoService.CreateTodoAsync(todo);
            return Created();
        }
    }
}
