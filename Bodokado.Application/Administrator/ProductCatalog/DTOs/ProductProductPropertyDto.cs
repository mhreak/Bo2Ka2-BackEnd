public class ProductProductPropertyDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public Guid? ProductPropertyId { get; set; }
    public string? ProductPropertyName { get; set; }
    public string? Value { get; set; }
    public DateTime CreatedAt { get; set; }
}