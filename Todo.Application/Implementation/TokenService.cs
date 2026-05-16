using Todo.Application.Constants;
using Todo.Application.Contracts;
using Todo.Application.DTOs.Request;
using Todo.Application.DTOs.Response;
using Todo.Application.Exceptions;
using Todo.Domain.RepositoryInterface;

namespace Todo.Application.Implementation
{
    public class TokenService : ITokenService
    {
        private readonly IUserRepository _userRepository;

        public TokenService(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        public async Task<TokenResponseDto> GetTokenAsync(
            TokenRequestDto requestDto)
        {
            var userDomain = 
                await _userRepository.GetByEmailAsync(requestDto.userName);

            if (userDomain == null)
                throw new InvalidEmailException(ErrorConstants.InvalidEmail);

            throw new NotImplementedException();
        }
    }
}
