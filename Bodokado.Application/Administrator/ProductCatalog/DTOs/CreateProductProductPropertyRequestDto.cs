public class CreateProductProductPropertyRequestDto
{
    public Guid ProductId { get; set; }
    public Guid? ProductPropertyId { get; set; }
    public string? Value { get; set; }
}