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


    Task<List<ProductPropertyValueDto>> GetPropertyValuesAsync(Guid? productPropertyId = null, bool onlyActive = false, CancellationToken ct = default);
    Task<ProductPropertyValueDto> GetPropertyValueByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductPropertyValueDto> CreatePropertyValueAsync(CreateProductPropertyValueRequestDto request, CancellationToken ct = default);
    Task<ProductPropertyValueDto> UpdatePropertyValueAsync(Guid id, UpdateProductPropertyValueRequestDto request, CancellationToken ct = default);
    Task DeletePropertyValueAsync(Guid id, CancellationToken ct = default);


    Task<List<ProductProductPropertyDto>> GetProductPropertiesAsync(Guid productId, CancellationToken ct = default);
    Task<ProductProductPropertyDto> GetProductPropertyByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductProductPropertyDto> CreateProductPropertyAsync(CreateProductProductPropertyRequestDto request, CancellationToken ct = default);
    Task<ProductProductPropertyDto> UpdateProductPropertyAsync(Guid id, UpdateProductProductPropertyRequestDto request, CancellationToken ct = default);
    Task DeleteProductPropertyAsync(Guid id, CancellationToken ct = default);
    Task<List<ProductProductPropertyDto>> SetProductPropertiesAsync(Guid productId, SetProductPropertiesRequestDto request, CancellationToken ct = default);
}