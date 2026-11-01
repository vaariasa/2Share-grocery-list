using GroceryList.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GroceryList.Api.Middleware;

/// <summary>
/// Catches exceptions thrown anywhere in the request and turns them into a JSON error
/// response (ProblemDetails) with the right HTTP status code.
/// This keeps the controllers free of try/catch blocks.
/// </summary>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var statusCode = exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                NotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            // Unexpected errors are logged, and their details are not sent to the client.
            if (statusCode == StatusCodes.Status500InternalServerError)
                logger.LogError(exception, "Unhandled exception");

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = statusCode == StatusCodes.Status500InternalServerError
                    ? "An unexpected error occurred."
                    : exception.Message
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
        }
    }
}
