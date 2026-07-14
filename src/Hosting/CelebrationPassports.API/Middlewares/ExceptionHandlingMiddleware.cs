using System.Text.Json;
using CelebrationPassports.Application.Exceptions;
using FluentValidation;

namespace CelebrationPassports.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var (statusCode, message) = ex switch
            {
                DuplicateEmailException => (StatusCodes.Status409Conflict, ex.Message),
                ConflictException => (StatusCodes.Status409Conflict, ex.Message),
                NotFoundException => (StatusCodes.Status404NotFound, ex.Message),
                ForbiddenAccessException => (StatusCodes.Status403Forbidden, ex.Message),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, ex.Message),
                ValidationException => (StatusCodes.Status400BadRequest, ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(new { message }));
        }
    }
}
