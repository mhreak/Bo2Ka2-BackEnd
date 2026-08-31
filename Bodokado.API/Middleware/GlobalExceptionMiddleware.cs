using System.Net;
using System.Text.Json;
using Bodokado.API.Helpers;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Bodokado.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            var localizer = context.RequestServices.GetRequiredService<IResponseLocalizer>();
            var errorId = Guid.NewGuid().ToString();
            _logger.LogError(ex, "Exception caught by global fallback middleware. ErrorId: {ErrorId}", errorId);
            int statusCode;
            string message;
            object? errors = null;

            if (ex is BadRequestException badRequest)
            {
                statusCode = StatusCodes.Status400BadRequest;
                message = await localizer.LocalizeAsync(badRequest.Message, badRequest.Args);
            }
            else if (ex is UnauthorizedAccessException)
            {
                statusCode = StatusCodes.Status401Unauthorized;
                message = await localizer.LocalizeAsync(MessageKeys.InvalidCredentials);
            }
            else if (ex is KeyNotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
                message = await localizer.LocalizeAsync(ex.Message);
            }
            else
            {
                statusCode = StatusCodes.Status400BadRequest;
                message = $"An unexpected error occurred. Reference ID: {errorId}";
                errors = ErrorSanitizer.Sanitize(ex, errorId);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            var responsePayload = statusCode switch
            {
                StatusCodes.Status400BadRequest => ApiResult.BadRequest(message, errors),
                StatusCodes.Status401Unauthorized => ApiResult.Unauthorized(message),
                StatusCodes.Status404NotFound => ApiResult.NotFound(message),
                _ => ApiResult.BadRequest(message, errors)
            };
            await context.Response.WriteAsJsonAsync(responsePayload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
