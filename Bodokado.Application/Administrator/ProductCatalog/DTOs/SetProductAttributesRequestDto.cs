public class SetProductAttributesRequestDto
{
    public List<ProductPropertyAssignmentDto> Items { get; set; } = new();
}

public class ProductPropertyAssignmentDto
{
    public Guid? ProductAttributeId { get; set; }
    public string? Value { get; set; }
}