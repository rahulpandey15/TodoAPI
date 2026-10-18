using Microsoft.AspNetCore.Mvc;
using Todo.Application.Common;

namespace Todo.API.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToProblemDetails(this Result result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Cannot convert a success result to a problem.");

            var statusCode = result.Error.Type switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };

            return new ObjectResult(new ProblemDetails
            {
                Status = statusCode,
                Title = result.Error.Code,
                Detail = result.Error.Description
            })
            {
                StatusCode = statusCode
            };
        }
    }
}
