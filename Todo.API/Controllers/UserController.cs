using Microsoft.AspNetCore.Mvc;
using Todo.Application.Contracts;
using Todo.Application.DTOs.Request;

namespace Todo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateUserDto request)
        {
            var response = await userService.CreateUserAsync(request);
            return Created(); // 201
        }
    }
}
