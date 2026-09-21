using Bodokado.Application.App.AdminModule.ProductCatalog.DTOs;

namespace Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;

public interface IProductCatalogService
{
    // Category
    Task<List<ProductCategoryDto>> GetCategoriesAsync(bool onlyActive = false, bool asTree = true, CancellationToken ct = default);
    Task<ProductCategoryDto> GetCategoryByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductCategoryDto> CreateCategoryAsync(CreateProductCategoryRequestDto request, CancellationToken ct = default);
    Task<ProductCategoryDto> UpdateCategoryAsync(Guid id, UpdateProductCategoryRequestDto request, CancellationToken ct = default);
    Task DeleteCategoryAsync(Guid id, CancellationToken ct = default);

    // Attribute
    Task<List<ProductAttributeDto>> GetAttributesAsync(Guid? productCategoryId = null, CancellationToken ct = default);
    Task<ProductAttributeDto> GetAttributeByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductAttributeDto> CreateAttributeAsync(CreateProductAttributeRequestDto request, CancellationToken ct = default);
    Task<ProductAttributeDto> UpdateAttributeAsync(Guid id, UpdateProductAttributeRequestDto request, CancellationToken ct = default);
    Task DeleteAttributeAsync(Guid id, CancellationToken ct = default);

    // AttributeValue
    Task<List<ProductAttributeValueDto>> GetAttributeValuesAsync(Guid? productAttributeId = null, bool onlyActive = false, CancellationToken ct = default);
    Task<ProductAttributeValueDto> GetAttributeValueByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductAttributeValueDto> CreateAttributeValueAsync(CreateProductAttributeValueRequestDto request, CancellationToken ct = default);
    Task<ProductAttributeValueDto> UpdateAttributeValueAsync(Guid id, UpdateProductAttributeValueRequestDto request, CancellationToken ct = default);
    Task DeleteAttributeValueAsync(Guid id, CancellationToken ct = default);

    // ProductProductAttribute
    Task<List<ProductProductAttributeDto>> GetProductAttributesAsync(Guid productId, CancellationToken ct = default);
    Task<ProductProductAttributeDto> GetProductAttributeByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductProductAttributeDto> CreateProductAttributeAsync(CreateProductProductAttributeRequestDto request, CancellationToken ct = default);
    Task<ProductProductAttributeDto> UpdateProductAttributeAsync(Guid id, UpdateProductProductAttributeRequestDto request, CancellationToken ct = default);
    Task DeleteProductAttributeAsync(Guid id, CancellationToken ct = default);
    Task<List<ProductProductAttributeDto>> SetProductAttributesAsync(Guid productId, SetProductAttributesRequestDto request, CancellationToken ct = default);
}