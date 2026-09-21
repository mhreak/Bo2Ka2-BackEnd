public class ProductProductAttributeDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public Guid? ProductAttributeId { get; set; }
    public string? ProductAttributeName { get; set; }
    public string? Value { get; set; }
    public DateTime CreatedAt { get; set; }
}