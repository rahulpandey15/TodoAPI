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
using Todo.Domain.DomainEntities;
using Todo.Domain.RepositoryInterface;

namespace Todo.Application.Implementation
{
    public class TokenService : ITokenService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;

        public TokenService(IUserRepository userRepository,
            IConfiguration configuration,
            IPasswordHasher passwordHasher)
        {
            this._userRepository = userRepository;
            this._configuration = configuration;
            this._passwordHasher = passwordHasher;
        }

        public async Task<TokenResponseDto> GetTokenAsync(
            TokenRequestDto requestDto)
        {
            var userDomain =
                await _userRepository.GetByEmailAsync(requestDto.userName);

            if (userDomain == null)
                throw new InvalidEmailException(ErrorConstants.InvalidEmail);

            if (!_passwordHasher.VerifyPassword(requestDto.password, userDomain.PasswordHash))
                throw new InvalidEmailException(ErrorConstants.InvalidPassword);

            string token =
                GenerateToken(userDomain);

            return new TokenResponseDto(token, string.Empty);
        }


        private string GenerateToken(UserDomain userResponse)
        {
            string secretKey = _configuration["Jwt:Secret"]!;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor
                = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity([
                        new Claim(ClaimTypes.Name, userResponse.FullName),
                        new Claim(ClaimTypes.Email, userResponse.Email),
                        new Claim("UserId", userResponse.Id.ToString())
                    ]),
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:TokenExpiryInMinutes"])),
                    SigningCredentials = credentials,
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"]
                };

            var tokenHandler = new JsonWebTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return token;
        }
    }
}
