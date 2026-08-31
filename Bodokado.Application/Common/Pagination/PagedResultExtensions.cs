namespace Bodokado.Application.Common.Pagination;

public static class PagedResultExtensions
{
    public static PagedResult<T> ToPagedResult<T>(this IQueryable<T> source, PaginationQuery query)
    {
        var normalized = query.Normalize();
        var totalCount = source.Count();
        var items = source.Skip(normalized.Skip).Take(normalized.Take).ToList();
        return PagedResult<T>.Create(items, normalized, totalCount);
    }
}
