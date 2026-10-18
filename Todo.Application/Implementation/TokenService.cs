using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Todo.Application.Common;
using Todo.Application.Constants;
using Todo.Application.Contracts;
using Todo.Application.DTOs.Request;
using Todo.Application.DTOs.Response;
using Todo.Application.Exceptions;
using Todo.Application.Utilities;
using Todo.Domain.DomainEntities;
using Todo.Domain.RepositoryInterface;

namespace Todo.Application.Implementation
{
   
    public class TokenService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IConfiguration configuration,
        IPasswordHasher passwordHasher,
        ITokenRevocationService tokenRevocationService)
        : ITokenService
    {
        public async Task<Result<TokenResponseDto>> GetTokenAsync(TokenRequestDto requestDto)
        {
            var userDomain =
                await userRepository.GetByEmailAsync(requestDto.userName);

            if (userDomain == null)
                return Result.Failure<TokenResponseDto>(
                    Error.Unauthorized("Auth.InvalidEmail", ErrorConstants.InvalidEmail));

            if (!passwordHasher.VerifyPassword(requestDto.password, userDomain.PasswordHash))
                return Result.Failure<TokenResponseDto>(
                    Error.Unauthorized("Auth.InvalidPassword", ErrorConstants.InvalidPassword));
            
            // Generate refresh token
            var refreshToken = 
                await GenerateAndStoreRefreshTokenAsync(userDomain.Id, clientIp: null);

            // Generate access token
            string accessToken = GenerateAccessToken(userDomain, refreshToken.refreshTokenId);
            int accessTokenExpiryMinutes = GetAccessTokenExpiryMinutes();

            return Result.Success(new TokenResponseDto(accessToken, refreshToken.refreshToken));
        }

        public async Task<Result<TokenResponseDto>> RefreshTokenAsync(
            RefreshTokenRequestDto requestDto, string? clientIp = null)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(requestDto.refreshToken))
                return Result.Failure<TokenResponseDto>(
                    Error.Unauthorized("Auth.InvalidRefreshToken", ErrorConstants.InvalidRefreshToken));    

            // Get the stored token
            var storedToken = 
                await refreshTokenRepository.GetByRefreshTokenAsync(requestDto.refreshToken);

            if (storedToken == null)
                return Result.Failure<TokenResponseDto>(
                    Error.Unauthorized("Auth.InvalidRefreshToken", ErrorConstants.InvalidRefreshToken));    

            // Check if token is expired
            if (storedToken.IsExpired())
                return Result.Failure<TokenResponseDto>(
                    Error.Unauthorized("Auth.InvalidRefreshToken", ErrorConstants.ExpiredRefreshToken));    

            // Check if token is revoked
            if (storedToken.IsRevoked())
            {
                await RevokeAllUserTokensAsync(storedToken.UserId, clientIp);
                
                return Result.Failure<TokenResponseDto>(
                    Error.Unauthorized("Auth.InvalidRefreshToken",
                        "Refresh token has been revoked. All user tokens have been invalidated for security."));
            }
            // Get the user
            var user = await userRepository.GetByIdAsync(storedToken.UserId);

            if(user == null)
            {
                return Result.Failure<TokenResponseDto>(
                    Error.BadRequest("Invalid User",
                        "The user associated with the refresh token does not exist."));
            }

            // Generate new refresh token (rotation)
            var newRefreshToken = await GenerateAndStoreRefreshTokenAsync(user.Id, clientIp);

            // Generate new access token
            string newAccessToken = GenerateAccessToken(user,newRefreshToken.refreshTokenId);
            int accessTokenExpiryMinutes = GetAccessTokenExpiryMinutes();

            // Revoke the old refresh token and link it to the new one
            await RevokeOldRefreshTokenAsync(storedToken.Id, newRefreshToken.refreshToken);
            
            return  Result.Success(new TokenResponseDto(newAccessToken, newRefreshToken.refreshToken));
        }

       
        public async Task<Result> RevokeTokenAsync(string refreshToken, string? clientIp = null)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return Result.Failure(
                    Error.BadRequest("Auth.InvalidRefreshToken", ErrorConstants.InvalidRefreshToken));

            // Get the stored token
            var storedToken = await refreshTokenRepository.GetByRefreshTokenAsync(refreshToken);

            if (storedToken == null)
               return Result.Failure(
                    Error.BadRequest("Auth.InvalidRefreshToken", ErrorConstants.InvalidRefreshToken));

            if (storedToken.IsRevoked())
                return Result.Failure(
                    Error.BadRequest("Auth.InvalidRefreshToken", ErrorConstants.RevokedRefreshToken));

            // Create a domain object to update
            var tokenToRevoke = new RefreshTokenDomain
            {
                Id = storedToken.Id,
                UserId = storedToken.UserId,
                Token = storedToken.Token,
                ExpiresAt = storedToken.ExpiresAt,
                RevokedAt = DateTime.UtcNow,
                RevokedByIp = clientIp,
                CreatedByIp = storedToken.CreatedByIp,
                CreatedAt = storedToken.CreatedAt,
                CreatedBy = storedToken.CreatedBy,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = "system"
            };

            await refreshTokenRepository.UpdateAsync(tokenToRevoke);
            await refreshTokenRepository.CommitAsync();
            await tokenRevocationService.InvalidateSessionCacheAsync(storedToken.Id);

            return Result.Success();
        }

   
        public async Task<bool> RevokeAllUserTokensAsync(
            Guid userId, 
            string? clientIp = null)
        {
            int revokedCount = await refreshTokenRepository.RevokeAllUserTokensAsync(userId, clientIp);
            return revokedCount >= 0;
        }
      
        private async Task<(string refreshToken, Guid refreshTokenId)> GenerateAndStoreRefreshTokenAsync(
            Guid userId, string? clientIp)
        {
            // Generate the raw refresh token using utility
            string rawToken = RefreshTokenUtility.GenerateToken();

            // Create domain object
            var refreshTokenDomain = new RefreshTokenDomain
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = rawToken,
                ExpiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenExpiryDays()),
                RevokedAt = null,
                CreatedByIp = clientIp,
                RevokedByIp = null,
                ReplacedByTokenId = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                CreatedBy = "system",
                UpdatedBy = null
            };

            // Add to repository
            await refreshTokenRepository.AddAsync(refreshTokenDomain);
            await refreshTokenRepository.CommitAsync();

         // return thr raw token
            return (rawToken, refreshTokenDomain.Id);
        }

        
        private async Task RevokeOldRefreshTokenAsync(Guid oldTokenId, string newRawToken)
        {
            // Get the old token to revoke
            var oldToken = 
                await refreshTokenRepository.GetByIdWithDetailsAsync(oldTokenId);

            if (oldToken == null)
                return;

            // Find the new token by hash to get its ID
            var newToken = 
                await refreshTokenRepository.GetByRefreshTokenAsync(newRawToken);

            if (newToken == null)
                return;

            // Detach the old token entity from the context to avoid tracking conflicts
            await refreshTokenRepository.DetachAsync(oldToken.Id);

            // Update the old token to mark it as replaced
            var oldTokenToUpdate = new RefreshTokenDomain
            {
                Id = oldToken.Id,
                UserId = oldToken.UserId,
                Token = oldToken.Token,
                ExpiresAt = oldToken.ExpiresAt,
                RevokedAt = DateTime.UtcNow,
                RevokedByIp = oldToken.RevokedByIp,
                CreatedByIp = oldToken.CreatedByIp,
                ReplacedByTokenId = newToken.Id, // Link to the replacement token
                CreatedAt = oldToken.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = oldToken.CreatedBy,
                UpdatedBy = "system"
            };


            // Detach the old token entity from the context to avoid tracking conflicts
            await refreshTokenRepository.DetachAsync(oldToken.Id);
            await refreshTokenRepository.UpdateAsync(oldTokenToUpdate);
            await refreshTokenRepository.CommitAsync();
        }

      
        private string GenerateAccessToken(UserDomain userResponse, Guid sessionId)
        {
            string secretKey = configuration["Jwt:Secret"]!;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            int expiryMinutes = GetAccessTokenExpiryMinutes();

            var tokenDescriptor
                = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity([
                        new Claim(ClaimTypes.Name, userResponse.FullName),
                        new Claim(ClaimTypes.Email, userResponse.Email),
                        new Claim("UserId", userResponse.Id.ToString()),
                        new Claim("sid",sessionId.ToString())
                    ]),
                    Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                    SigningCredentials = credentials,
                    Issuer = configuration["Jwt:Issuer"],
                    Audience = configuration["Jwt:Audience"]
                };

            var tokenHandler = new JsonWebTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return token;
        }

  
        private int GetAccessTokenExpiryMinutes()
        {
            var configValue = configuration["Jwt:AccessTokenExpirationMinutes"];
            if (int.TryParse(configValue, out int minutes))
                return minutes;

            // Fallback to old configuration key for backward compatibility
            configValue = configuration["Jwt:TokenExpiryInMinutes"];
            if (int.TryParse(configValue, out int legacyMinutes))
                return legacyMinutes;

            return 60; // Default fallback
        }

    
        private int GetRefreshTokenExpiryDays()
        {
            var configValue = configuration["Jwt:RefreshTokenExpirationDays"];
            if (int.TryParse(configValue, out int days))
                return days;

            return 7; // Default fallback
        }
    }
}
