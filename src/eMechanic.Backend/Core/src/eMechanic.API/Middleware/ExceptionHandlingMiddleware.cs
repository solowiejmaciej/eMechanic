namespace eMechanic.API.Middleware;

using eMechanic.Common.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, error) = exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized,
                new Error(
                    EErrorCode.UnauthorizedError,
                    "In order to perform this operation please pass JWT token in Authorization header"
                )),
            BadHttpRequestException => (StatusCodes.Status400BadRequest,
                new Error(
                    EErrorCode.ValidationError,
                    "Something is wrong with json format"
                )),
            _ => (
                StatusCodes.Status500InternalServerError,
                new Error(
                    EErrorCode.InternalServerError,
                    "An internal server error occurred"
                )
            )
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new
        {
            error.Code,
            error.Message,
            error.ValidationErrors
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
