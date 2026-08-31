namespace Bodokado.Application.Common.Pagination;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int Page { get; set; } = PaginationQuery.DefaultPage;
    public int PageSize { get; set; } = PaginationQuery.DefaultPageSize;
    public int TotalCount { get; set; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
    public int FirstItemOnPage => TotalCount == 0 ? 0 : ((Page - 1) * PageSize) + 1;
    public int LastItemOnPage => TotalCount == 0 ? 0 : Math.Min(Page * PageSize, TotalCount);

    public static PagedResult<T> Create(IEnumerable<T> items, PaginationQuery pagination, int totalCount)
    {
        var normalized = pagination.Normalize();
        return new PagedResult<T> { Items = items, Page = normalized.Page, PageSize = normalized.PageSize, TotalCount = totalCount };
    }
}
