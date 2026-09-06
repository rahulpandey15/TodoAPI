using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
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
   
    public class TokenService : ITokenService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenRevocationService _tokenRevocationService;

        public TokenService(IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IConfiguration configuration,
            IPasswordHasher passwordHasher,
            ITokenRevocationService tokenRevocationService)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _tokenRevocationService = tokenRevocationService;
        }

    
        public async Task<TokenResponseDto> GetTokenAsync(TokenRequestDto requestDto)
        {
            var userDomain =
                await _userRepository.GetByEmailAsync(requestDto.userName);

            if (userDomain == null)
                throw new InvalidEmailException(ErrorConstants.InvalidEmail);

            if (!_passwordHasher.VerifyPassword(requestDto.password, userDomain.PasswordHash))
                throw new InvalidEmailException(ErrorConstants.InvalidPassword);


            // Generate refresh token
            var refreshToken = await GenerateAndStoreRefreshTokenAsync(userDomain.Id, clientIp: null);


            // Generate access token
            string accessToken = GenerateAccessToken(userDomain, refreshToken.refreshTokenId);
            int accessTokenExpiryMinutes = GetAccessTokenExpiryMinutes();

            return new TokenResponseDto(
                accessToken, refreshToken.refreshToken);
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(
            RefreshTokenRequestDto requestDto, string? clientIp = null)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(requestDto.refreshToken))
                throw new InvalidTokenException("Refresh token is required.");

            // Get the stored token
            var storedToken = 
                await _refreshTokenRepository.GetByRefreshTokenAsync(requestDto.refreshToken);

            if (storedToken == null)
            {
                throw new InvalidTokenException("Invalid refresh token.");
            }

            // Check if token is expired
            if (storedToken.IsExpired())
                throw new InvalidTokenException("Refresh token has expired.");

            // Check if token is revoked
            if (storedToken.IsRevoked())
            {
                await RevokeAllUserTokensAsync(storedToken.UserId, clientIp);

                // TODO : write a logic to remove all refresh token from redis as well

                throw new InvalidTokenException(
                    "Refresh token has been revoked. All user tokens have been invalidated for security.");
            }

            // Get the user
            var user = await _userRepository.GetByIdAsync(storedToken.UserId);

            if (user == null)
                throw new InvalidEmailException("User not found.");

            // Generate new refresh token (rotation)
            var newRefreshToken = await GenerateAndStoreRefreshTokenAsync(user.Id, clientIp);

            // Generate new access token
            string newAccessToken = GenerateAccessToken(user,newRefreshToken.refreshTokenId);
            int accessTokenExpiryMinutes = GetAccessTokenExpiryMinutes();

            // Revoke the old refresh token and link it to the new one
            await RevokeOldRefreshTokenAsync(storedToken.Id, newRefreshToken.refreshToken);

            return new TokenResponseDto(newAccessToken, newRefreshToken.refreshToken);
        }

       
        public async Task<bool> RevokeTokenAsync(
            string refreshToken, string? clientIp = null)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return false;

            // Get the stored token
            var storedToken = await _refreshTokenRepository.GetByRefreshTokenAsync(refreshToken);

            if (storedToken == null)
                return false;

            if (storedToken.IsRevoked())
                return true; // Already revoked

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

            await _refreshTokenRepository.UpdateAsync(tokenToRevoke);
            await _refreshTokenRepository.CommitAsync();
            await _tokenRevocationService.InvalidateSessionCacheAsync(storedToken.Id);

            return true;
        }

   
        public async Task<bool> RevokeAllUserTokensAsync(
            Guid userId, 
            string? clientIp = null)
        {
            int revokedCount = await _refreshTokenRepository.RevokeAllUserTokensAsync(userId, clientIp);
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
            await _refreshTokenRepository.AddAsync(refreshTokenDomain);
            await _refreshTokenRepository.CommitAsync();

         // return thr raw token
            return (rawToken, refreshTokenDomain.Id);
        }

        
        private async Task RevokeOldRefreshTokenAsync(Guid oldTokenId, string newRawToken)
        {
            // Get the old token to revoke
            var oldToken = 
                await _refreshTokenRepository.GetByIdWithDetailsAsync(oldTokenId);

            if (oldToken == null)
                return;

            // Find the new token by hash to get its ID
            var newToken = 
                await _refreshTokenRepository.GetByRefreshTokenAsync(newRawToken);

            if (newToken == null)
                return;

            // Detach the old token entity from the context to avoid tracking conflicts
            await _refreshTokenRepository.DetachAsync(oldToken.Id);

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
            await _refreshTokenRepository.DetachAsync(oldToken.Id);
            await _refreshTokenRepository.UpdateAsync(oldTokenToUpdate);
            await _refreshTokenRepository.CommitAsync();
        }

      
        private string GenerateAccessToken(UserDomain userResponse, Guid sessionId)
        {
            string secretKey = _configuration["Jwt:Secret"]!;

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
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"]
                };

            var tokenHandler = new JsonWebTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return token;
        }

  
        private int GetAccessTokenExpiryMinutes()
        {
            var configValue = _configuration["Jwt:AccessTokenExpirationMinutes"];
            if (int.TryParse(configValue, out int minutes))
                return minutes;

            // Fallback to old configuration key for backward compatibility
            configValue = _configuration["Jwt:TokenExpiryInMinutes"];
            if (int.TryParse(configValue, out int legacyMinutes))
                return legacyMinutes;

            return 60; // Default fallback
        }

    
        private int GetRefreshTokenExpiryDays()
        {
            var configValue = _configuration["Jwt:RefreshTokenExpirationDays"];
            if (int.TryParse(configValue, out int days))
                return days;

            return 7; // Default fallback
        }
    }
}
