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
        private readonly ILogger<TodoController> logger;

        public TodoController(ITodoService todoService, ILogger<TodoController> logger)
        {
            this._todoService = todoService;
            this.logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            logger.LogInformation($"Executing GET method inside a TodoController at {DateTime.Now}");
            var todoList = await _todoService.GetItems();
            logger.LogInformation($"Execution of  GET method inside a TodoController completed at {DateTime.Now}");
            return Ok(todoList);
        }


        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody] CreateTodoDto todo)
        {
            logger.LogInformation("Executing POST method inside a TodoController at {0}", DateTime.Now);
            var created = await _todoService.CreateTodoAsync(todo);
            return Created();
        }
    }
}
