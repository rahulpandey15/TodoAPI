using Todo.Application.DTOs.Request;
using Todo.Application.DTOs.Response;

namespace Todo.Application.Contracts;

public interface ITokenService
{
    
    Task<TokenResponseDto> GetTokenAsync(TokenRequestDto requestDto);

    Task<TokenResponseDto> RefreshTokenAsync(
        RefreshTokenRequestDto requestDto, string? clientIp = null);

    Task<bool> RevokeTokenAsync(string refreshToken, string? clientIp = null);

    Task<bool> RevokeAllUserTokensAsync(Guid userId, string? clientIp = null);
}
