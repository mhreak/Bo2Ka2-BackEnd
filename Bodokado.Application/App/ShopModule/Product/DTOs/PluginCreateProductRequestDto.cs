public class PluginCreateProductRequestDto
{
    public string? ExternalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Brand { get; set; }

    public decimal BasePrice { get; set; }
    public bool IsDiscountEnabled { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool HasSpecialPackaging { get; set; }
    public bool IsSpecial { get; set; }
    public bool Publish { get; set; } = true;

    public decimal? WeightGrams { get; set; }
    public decimal? LengthCm { get; set; }
    public decimal? WidthCm { get; set; }
    public decimal? HeightCm { get; set; }

    public string? MainImageUrl { get; set; }
    public List<string>? ImageUrls { get; set; }
}

public class PluginCreateProductsBatchRequestDto
{
    public List<PluginCreateProductRequestDto> Products { get; set; } = new();
}

public class PluginCreateProductResultDto
{
    public string? ExternalId { get; set; }
    public Guid? ProductId { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}

public class PluginCreateProductsBatchResponseDto
{
    public int Total { get; set; }
    public int Succeeded { get; set; }
    public int Failed { get; set; }
    public List<PluginCreateProductResultDto> Results { get; set; } = new();
}