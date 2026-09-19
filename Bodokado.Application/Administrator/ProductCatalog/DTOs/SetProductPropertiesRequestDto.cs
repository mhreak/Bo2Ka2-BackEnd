public class SetProductPropertiesRequestDto
{
    public List<ProductPropertyAssignmentDto> Items { get; set; } = new();
}

public class ProductPropertyAssignmentDto
{
    public Guid? ProductPropertyId { get; set; }
    public string? Value { get; set; }
}