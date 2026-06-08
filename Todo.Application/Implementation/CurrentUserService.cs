using Microsoft.AspNetCore.Http;
using Todo.Application.Contracts;

namespace Todo.Application.Implementation
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUserId()
        {
            var userId
                  = _httpContextAccessor.HttpContext.Request.Headers["CurrentUserId"];

            if (!string.IsNullOrWhiteSpace(userId))
                return userId;

            return string.Empty;
        }
    }
}
