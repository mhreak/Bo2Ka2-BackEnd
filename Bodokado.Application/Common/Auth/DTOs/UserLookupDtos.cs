namespace Bodokado.Application.Administrator.UserLookup;

public class UserLookupRequestDto
{
    public string? SearchTerm { get; set; }
    public string? Role { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class UserLookupDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
