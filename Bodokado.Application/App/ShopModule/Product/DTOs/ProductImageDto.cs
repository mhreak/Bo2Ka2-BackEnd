namespace Bodokado.Application.App.ShopModule.Products.DTOs;

/// <summary>اطلاعات تصویر محصول که از روی FileAsset (API آپلود عمومی) resolve می‌شود.</summary>
public class ProductImageDto
{
    public Guid FileAssetId { get; set; }
    public string? Path { get; set; }
    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
}
