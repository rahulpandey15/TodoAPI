using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Todo.Application.DTOs.Response;

namespace Todo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodoController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<TodoResponseDto> todoList = [];

            todoList.Add(new TodoResponseDto(Name: "Start Learning Langchain", IsCompleted: true));
            todoList.Add(new TodoResponseDto(Name: "Start Learning VectorDb", IsCompleted: true));


            return Ok(todoList);

        }
    }
}
