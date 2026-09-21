public class CreateProductProductAttributeRequestDto
{
    public Guid ProductId { get; set; }
    public Guid? ProductAttributeId { get; set; }
    public string? Value { get; set; }
}