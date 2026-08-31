namespace Bodokado.Application.Common.Responses;

public class MetaData
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
    public int FirstItemOnPage { get; set; }
    public int LastItemOnPage { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
}
