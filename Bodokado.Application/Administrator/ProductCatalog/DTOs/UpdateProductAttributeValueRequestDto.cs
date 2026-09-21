public class UpdateProductAttributeValueRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public Guid ProductAttributeId { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}