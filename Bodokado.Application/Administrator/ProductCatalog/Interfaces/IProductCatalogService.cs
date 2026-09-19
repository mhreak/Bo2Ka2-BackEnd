using Bodokado.Application.App.AdminModule.ProductCatalog.DTOs;

namespace Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;

public interface IProductCatalogService
{
    Task<List<ProductCategoryDto>> GetCategoriesAsync(bool onlyActive = false, bool asTree = true, CancellationToken ct = default);
    Task<ProductCategoryDto> GetCategoryByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductCategoryDto> CreateCategoryAsync(CreateProductCategoryRequestDto request, CancellationToken ct = default);
    Task<ProductCategoryDto> UpdateCategoryAsync(Guid id, UpdateProductCategoryRequestDto request, CancellationToken ct = default);
    Task DeleteCategoryAsync(Guid id, CancellationToken ct = default);

    Task<List<ProductPropertyDto>> GetPropertiesAsync(Guid? productCategoryId = null, CancellationToken ct = default);
    Task<ProductPropertyDto> GetPropertyByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductPropertyDto> CreatePropertyAsync(CreateProductPropertyRequestDto request, CancellationToken ct = default);
    Task<ProductPropertyDto> UpdatePropertyAsync(Guid id, UpdateProductPropertyRequestDto request, CancellationToken ct = default);
    Task DeletePropertyAsync(Guid id, CancellationToken ct = default);
}