using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Todo.Application.DTO;
using Todo.Application.Interface;

namespace Todo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly IUserService userService;

        public TodoController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserDto request)
        {
            var data = await userService.AddUser(request);
            return Ok();
        }
    }
}
