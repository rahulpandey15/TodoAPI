using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace Todo.API.Middlewares
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;

        public UserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            var token = await context.GetTokenAsync("access_token");

            if (string.IsNullOrWhiteSpace(token))
            {
                await _next(context); // don't break if token doesn't exist
            }
            else
            {
                var currentUserId = GetUserId(token);
                
                context.Request.Headers.Add("CurrentUserId", currentUserId.ToString());

                await _next(context);
            }
        }


        private Guid GetUserId(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadJwtToken(token);

            var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value;
            return Guid.Parse(userId);
        }

    }
}
