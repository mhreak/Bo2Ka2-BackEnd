public class ProductPropertyValueDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public Guid ProductPropertyId { get; set; }
    public string? ProductPropertyName { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}