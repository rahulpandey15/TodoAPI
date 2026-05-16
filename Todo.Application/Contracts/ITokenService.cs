using Todo.Application.DTOs.Request;
using Todo.Application.DTOs.Response;

namespace Todo.Application.Contracts;

public interface ITokenService
{
    Task<TokenResponseDto> GetTokenAsync(TokenRequestDto requestDto);
}
