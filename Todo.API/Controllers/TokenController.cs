using Microsoft.AspNetCore.Mvc;
using Todo.Application.Contracts;
using Todo.Application.DTOs.Request;

namespace Todo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController(ITokenService _tokenService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Get(TokenRequestDto requestDto)
        {
            var response =
                await _tokenService.GetTokenAsync(requestDto);

            return Ok();
        }

    }
}
