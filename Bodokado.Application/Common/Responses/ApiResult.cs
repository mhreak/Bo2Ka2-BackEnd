namespace Bodokado.Application.Common.Responses;

public class ApiResult<T>
{
    public int Status { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public MetaData? Meta { get; set; }
    public object? Errors { get; set; }
    public static ApiResult<T> Ok(T data) => new() { Status = 200, Success = true, Data = data };
}
