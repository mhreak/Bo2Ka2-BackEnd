namespace Bodokado.Application.Common.Pagination;

public class PaginationQuery
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public int Page { get; set; } = DefaultPage;
    public int PageSize { get; set; } = DefaultPageSize;

    [System.Text.Json.Serialization.JsonIgnore]
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever]
    public int Skip => (Page - 1) * PageSize;

    [System.Text.Json.Serialization.JsonIgnore]
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever]
    public int Take => PageSize;

    public PaginationQuery Normalize()
    {
        var page = Page < 1 ? DefaultPage : Page;
        var pageSize = PageSize <= 0 ? DefaultPageSize : Math.Min(PageSize, MaxPageSize);
        return new PaginationQuery { Page = page, PageSize = pageSize };
    }
}
