using Microsoft.AspNetCore.Mvc;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Pagination;
using Bodokado.Application.Common.Responses;

namespace Bodokado.API.Helpers;

public static class ApiResult
{
    public static ApiResult<object?> Success(string message)
    {
        return new ApiResult<object?> { Status = StatusCodes.Status200OK, Success = true, Message = message, Data = null };
    }

    public static ApiResult<T> Success<T>(T data, string message)
    {
        return new ApiResult<T> { Status = StatusCodes.Status200OK, Success = true, Message = message, Data = data };
    }

    public static ApiResult<IEnumerable<T>> Success<T>(PagedResult<T> pagedResult, string message)
    {
        return new ApiResult<IEnumerable<T>>
        {
            Status = StatusCodes.Status200OK,
            Success = true,
            Message = message,
            Data = pagedResult.Items,
            Meta = new MetaData
            {
                Page = pagedResult.Page, PageSize = pagedResult.PageSize, TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages, HasPrevious = pagedResult.HasPreviousPage, HasNext = pagedResult.HasNextPage,
                FirstItemOnPage = pagedResult.FirstItemOnPage, LastItemOnPage = pagedResult.LastItemOnPage
            }
        };
    }

    public static ApiResult<object> Created(string message)
    {
        return new ApiResult<object> { Status = StatusCodes.Status201Created, Success = true, Message = message };
    }

    public static ApiResult<object> BadRequest(string message, object? errors = null)
    {
        return new ApiResult<object> { Status = StatusCodes.Status400BadRequest, Success = false, Message = message, Errors = errors };
    }

    public static ApiResult<object> Unauthorized(string message)
    {
        return new ApiResult<object> { Status = StatusCodes.Status401Unauthorized, Success = false, Message = message };
    }

    public static ApiResult<object> Forbidden(string message)
    {
        return new ApiResult<object> { Status = StatusCodes.Status403Forbidden, Success = false, Message = message };
    }

    public static ApiResult<object> NotFound(string message)
    {
        return new ApiResult<object> { Status = StatusCodes.Status404NotFound, Success = false, Message = message };
    }

    public static ApiResult<object> Conflict(string message, object? errors = null)
    {
        return new ApiResult<object> { Status = StatusCodes.Status409Conflict, Success = false, Message = message, Errors = errors };
    }

    public static ApiResult<object> Error(string message)
    {
        return new ApiResult<object> { Status = StatusCodes.Status500InternalServerError, Success = false, Message = message };
    }
}
