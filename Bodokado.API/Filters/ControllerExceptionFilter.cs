using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Bodokado.API.Helpers;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bodokado.API.Filters;

public class ControllerExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        var localizer = context.HttpContext.RequestServices.GetRequiredService<IResponseLocalizer>();
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ControllerExceptionFilterAttribute>>();
        var errorId = Guid.NewGuid().ToString();
        logger.LogError(context.Exception, "Exception caught by controller exception filter. ErrorId: {ErrorId}", errorId);

        int statusCode;
        string message;
        object? errors = null;

        if (context.Exception is BadRequestException badRequest)
        {
            statusCode = StatusCodes.Status400BadRequest;
            message = localizer.LocalizeAsync(badRequest.Message, badRequest.Args).GetAwaiter().GetResult();
        }
        else if (context.Exception is UnauthorizedAccessException)
        {
            statusCode = StatusCodes.Status401Unauthorized;
            message = localizer.LocalizeAsync(MessageKeys.InvalidCredentials).GetAwaiter().GetResult();
        }
        else if (context.Exception is KeyNotFoundException)
        {
            statusCode = StatusCodes.Status404NotFound;
            message = localizer.LocalizeAsync(context.Exception.Message).GetAwaiter().GetResult();
        }
        else
        {
            statusCode = StatusCodes.Status400BadRequest;
            message = $"An unexpected error occurred. Reference ID: {errorId}";
            errors = ErrorSanitizer.Sanitize(context.Exception, errorId);
        }

        var responsePayload = statusCode switch
        {
            StatusCodes.Status400BadRequest => ApiResult.BadRequest(message, errors),
            StatusCodes.Status401Unauthorized => ApiResult.Unauthorized(message),
            StatusCodes.Status404NotFound => ApiResult.NotFound(message),
            _ => ApiResult.BadRequest(message, errors)
        };

        context.Result = new ObjectResult(responsePayload)
        {
            StatusCode = statusCode
        };
    }
}