using Microsoft.AspNetCore.Mvc;
using Bodokado.Application.Common.Exceptions;

namespace Bodokado.API.Helpers;

public static class ErrorSanitizer
{
    public static object? Sanitize(Exception ex, string errorId)
    {
        return new { errorId, message = ex.Message };
    }
}
