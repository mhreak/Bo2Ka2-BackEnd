using Bodokado.Application.App.AdminModule.ProductCatalog.DTOs;
using Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;
using Bodokado.Application.App.ShopModule.Products.Interfaces; // یا مسیر درست IProductRepository
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.File.Interfaces;
using Bodokado.Application.Common.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Entities.Products;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.Administrator.ProductCatalog.Services;

public class ProductCatalogService : IProductCatalogService
{
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly IProductAttributeRepository _attributeRepository;
    private readonly IProductAttributeValueRepository _attributeValueRepository;
    private readonly IProductProductAttributeRepository _productProductAttributeRepository;
    private readonly IProductRepository _productRepository;
    private readonly IFileAssetRepository _fileAssetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductCatalogService(
        IProductCategoryRepository categoryRepository,
        IProductAttributeRepository attributeRepository,
        IProductAttributeValueRepository attributeValueRepository,
        IProductProductAttributeRepository productProductAttributeRepository,
        IProductRepository productRepository,
        IFileAssetRepository fileAssetRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _attributeRepository = attributeRepository;
        _attributeValueRepository = attributeValueRepository;
        _productProductAttributeRepository = productProductAttributeRepository;
        _productRepository = productRepository;
        _fileAssetRepository = fileAssetRepository;
        _unitOfWork = unitOfWork;
    }

    // ───────────── Category ─────────────

    public async Task<List<ProductCategoryDto>> GetCategoriesAsync(bool onlyActive = false, bool asTree = true, CancellationToken ct = default)
    {
        var all = await _categoryRepository.GetAllWithDetailsAsync(onlyActive, ct);
        var dtos = all.Select(MapCategoryFlat).ToList();

        if (!asTree)
            return dtos;

        var lookup = dtos.ToDictionary(c => c.Id);
        var roots = new List<ProductCategoryDto>();
        foreach (var dto in dtos)
        {
            if (dto.ParentCategoryId is Guid parentId && lookup.TryGetValue(parentId, out var parent))
                parent.Children.Add(dto);
            else
                roots.Add(dto);
        }
        return roots;
    }

    public async Task<ProductCategoryDto> GetCategoryByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _categoryRepository.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductCategoryNotFound, "product_category_not_found");
        return MapCategoryFlat(entity);
    }

    public async Task<ProductCategoryDto> CreateCategoryAsync(CreateProductCategoryRequestDto request, CancellationToken ct = default)
    {
        await ValidateParentAndImageAsync(request.ParentCategoryId, request.ImageId, excludeId: null, ct);

        var entity = new ProductCategory
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            IsActive = request.IsActive,
            ParentCategoryId = request.ParentCategoryId,
            ImageId = request.ImageId,
            CreatedAt = DateTime.UtcNow
        };

        await _categoryRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        var created = await _categoryRepository.GetByIdWithDetailsAsync(entity.Id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductCategoryNotFound, "product_category_not_found");
        return MapCategoryFlat(created);
    }

    public async Task<ProductCategoryDto> UpdateCategoryAsync(Guid id, UpdateProductCategoryRequestDto request, CancellationToken ct = default)
    {
        var entity = await _categoryRepository.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductCategoryNotFound, "product_category_not_found");

        if (request.ParentCategoryId == id)
            throw new BadRequestException(MessageKeys.ProductCategoryInvalidParent, "product_category_invalid_parent");

        if (request.ParentCategoryId.HasValue)
        {
            var isCycle = await _categoryRepository.IsDescendantOfAsync(request.ParentCategoryId.Value, id, ct);
            if (isCycle)
                throw new BadRequestException(MessageKeys.ProductCategoryInvalidParent, "product_category_invalid_parent");
        }

        await ValidateParentAndImageAsync(request.ParentCategoryId, request.ImageId, excludeId: id, ct);

        entity.Name = request.Name.Trim();
        entity.IsActive = request.IsActive;
        entity.ParentCategoryId = request.ParentCategoryId;
        entity.ImageId = request.ImageId;
        entity.UpdatedAt = DateTime.UtcNow;

        _categoryRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _categoryRepository.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductCategoryNotFound, "product_category_not_found");
        return MapCategoryFlat(updated);
    }

    public async Task DeleteCategoryAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _categoryRepository.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductCategoryNotFound, "product_category_not_found");

        if (await _categoryRepository.HasChildrenAsync(id, ct))
            throw new BadRequestException(MessageKeys.ProductCategoryHasChildren, "product_category_has_children");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _categoryRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    // ───────────── Attribute ─────────────

    public async Task<List<ProductAttributeDto>> GetAttributesAsync(Guid? productCategoryId = null, CancellationToken ct = default)
    {
        var list = await _attributeRepository.GetListAsync(productCategoryId, ct);
        return list.Select(MapAttribute).ToList();
    }

    public async Task<ProductAttributeDto> GetAttributeByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _attributeRepository.GetByIdWithCategoryAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductAttributeNotFound, "product_attribute_not_found");
        return MapAttribute(entity);
    }

    public async Task<ProductAttributeDto> CreateAttributeAsync(CreateProductAttributeRequestDto request, CancellationToken ct = default)
    {
        await ValidateCategoryExistsIfSetAsync(request.ProductCategoryId, ct);
        ValidateAttributeType(request.Type);

        var entity = new ProductAttribute
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Type = request.Type,
            SortOrder = request.SortOrder,
            ProductCategoryId = request.ProductCategoryId,
            CreatedAt = DateTime.UtcNow
        };

        await _attributeRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        var created = await _attributeRepository.GetByIdWithCategoryAsync(entity.Id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductAttributeNotFound, "product_attribute_not_found");
        return MapAttribute(created);
    }

    public async Task<ProductAttributeDto> UpdateAttributeAsync(Guid id, UpdateProductAttributeRequestDto request, CancellationToken ct = default)
    {
        var entity = await _attributeRepository.GetByIdWithCategoryAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductAttributeNotFound, "product_attribute_not_found");

        await ValidateCategoryExistsIfSetAsync(request.ProductCategoryId, ct);
        ValidateAttributeType(request.Type);

        entity.Name = request.Name.Trim();
        entity.Type = request.Type;
        entity.SortOrder = request.SortOrder;
        entity.ProductCategoryId = request.ProductCategoryId;
        entity.UpdatedAt = DateTime.UtcNow;

        _attributeRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _attributeRepository.GetByIdWithCategoryAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductAttributeNotFound, "product_attribute_not_found");
        return MapAttribute(updated);
    }

    public async Task DeleteAttributeAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _attributeRepository.GetByIdWithCategoryAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductAttributeNotFound, "product_attribute_not_found");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _attributeRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    // ───────────── AttributeValue ─────────────

    public async Task<List<ProductAttributeValueDto>> GetAttributeValuesAsync(Guid? productAttributeId = null, bool onlyActive = false, CancellationToken ct = default)
    {
        var list = await _attributeValueRepository.GetListAsync(productAttributeId, onlyActive, ct);
        return list.Select(MapAttributeValue).ToList();
    }

    public async Task<ProductAttributeValueDto> GetAttributeValueByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _attributeValueRepository.GetByIdWithAttributeAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductAttributeValueNotFound, "product_attribute_value_not_found");
        return MapAttributeValue(entity);
    }

    public async Task<ProductAttributeValueDto> CreateAttributeValueAsync(CreateProductAttributeValueRequestDto request, CancellationToken ct = default)
    {
        await EnsureAttributeExistsAsync(request.ProductAttributeId, ct);

        var entity = new ProductAttributeValue
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Value = request.Value.Trim(),
            ProductAttributeId = request.ProductAttributeId,
            IsActive = request.IsActive,
            SortOrder = request.SortOrder,
            CreatedAt = DateTime.UtcNow
        };

        await _attributeValueRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        var created = await _attributeValueRepository.GetByIdWithAttributeAsync(entity.Id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductAttributeValueNotFound, "product_attribute_value_not_found");
        return MapAttributeValue(created);
    }

    public async Task<ProductAttributeValueDto> UpdateAttributeValueAsync(Guid id, UpdateProductAttributeValueRequestDto request, CancellationToken ct = default)
    {
        var entity = await _attributeValueRepository.GetByIdWithAttributeAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductAttributeValueNotFound, "product_attribute_value_not_found");

        await EnsureAttributeExistsAsync(request.ProductAttributeId, ct);

        entity.Title = request.Title.Trim();
        entity.Value = request.Value.Trim();
        entity.ProductAttributeId = request.ProductAttributeId;
        entity.IsActive = request.IsActive;
        entity.SortOrder = request.SortOrder;
        entity.UpdatedAt = DateTime.UtcNow;

        _attributeValueRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _attributeValueRepository.GetByIdWithAttributeAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductAttributeValueNotFound, "product_attribute_value_not_found");
        return MapAttributeValue(updated);
    }

    public async Task DeleteAttributeValueAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _attributeValueRepository.GetByIdWithAttributeAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductAttributeValueNotFound, "product_attribute_value_not_found");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _attributeValueRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    // ───────────── ProductProductAttribute ─────────────

    public async Task<List<ProductProductAttributeDto>> GetProductAttributesAsync(Guid productId, CancellationToken ct = default)
    {
        await EnsureProductExistsAsync(productId, ct);
        var list = await _productProductAttributeRepository.GetByProductIdAsync(productId, ct);
        return list.Select(MapProductAttribute).ToList();
    }

    public async Task<ProductProductAttributeDto> GetProductAttributeByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _productProductAttributeRepository.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductProductAttributeNotFound, "product_product_attribute_not_found");
        return MapProductAttribute(entity);
    }

    public async Task<ProductProductAttributeDto> CreateProductAttributeAsync(CreateProductProductAttributeRequestDto request, CancellationToken ct = default)
    {
        await EnsureProductExistsAsync(request.ProductId, ct);
        if (request.ProductAttributeId.HasValue)
            await EnsureAttributeExistsAsync(request.ProductAttributeId.Value, ct);

        ValidateProductAttributeValue(request.Value);

        var entity = new ProductProductAttribute
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            ProductAttributeId = request.ProductAttributeId,
            Value = request.Value?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _productProductAttributeRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        var created = await _productProductAttributeRepository.GetByIdWithDetailsAsync(entity.Id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductProductAttributeNotFound, "product_product_attribute_not_found");
        return MapProductAttribute(created);
    }

    public async Task<ProductProductAttributeDto> UpdateProductAttributeAsync(Guid id, UpdateProductProductAttributeRequestDto request, CancellationToken ct = default)
    {
        var entity = await _productProductAttributeRepository.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductProductAttributeNotFound, "product_product_attribute_not_found");

        if (request.ProductAttributeId.HasValue)
            await EnsureAttributeExistsAsync(request.ProductAttributeId.Value, ct);

        ValidateProductAttributeValue(request.Value);

        entity.ProductAttributeId = request.ProductAttributeId;
        entity.Value = request.Value?.Trim();
        entity.UpdatedAt = DateTime.UtcNow;

        _productProductAttributeRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _productProductAttributeRepository.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductProductAttributeNotFound, "product_product_attribute_not_found");
        return MapProductAttribute(updated);
    }

    public async Task DeleteProductAttributeAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _productProductAttributeRepository.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductProductAttributeNotFound, "product_product_attribute_not_found");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _productProductAttributeRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<List<ProductProductAttributeDto>> SetProductAttributesAsync(Guid productId, SetProductAttributesRequestDto request, CancellationToken ct = default)
    {
        await EnsureProductExistsAsync(productId, ct);

        foreach (var item in request.Items)
        {
            if (item.ProductAttributeId.HasValue)
                await EnsureAttributeExistsAsync(item.ProductAttributeId.Value, ct);
            ValidateProductAttributeValue(item.Value);
        }

        await _productProductAttributeRepository.SoftDeleteByProductIdAsync(productId, ct);

        foreach (var item in request.Items)
        {
            // مهم: ProductProductAttribute نه ProductAttributeValue
            await _productProductAttributeRepository.AddAsync(new ProductProductAttribute
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                ProductAttributeId = item.ProductAttributeId,
                Value = item.Value?.Trim(),
                CreatedAt = DateTime.UtcNow
            });
        }

        await _unitOfWork.SaveChangesAsync(ct);
        return await GetProductAttributesAsync(productId, ct);
    }

    // ───────────── Helpers ─────────────

    private async Task ValidateParentAndImageAsync(Guid? parentCategoryId, Guid? imageId, Guid? excludeId, CancellationToken ct)
    {
        if (parentCategoryId.HasValue)
        {
            var parent = await _categoryRepository.GetByIdAsync(parentCategoryId.Value);
            if (parent is null || parent.IsDeleted)
                throw new BadRequestException(MessageKeys.ProductCategoryNotFound, "product_category_not_found");
        }

        if (imageId.HasValue)
        {
            var file = await _fileAssetRepository.GetByIdAsync(imageId.Value);
            if (file is null || file.IsDeleted)
                throw new BadRequestException(MessageKeys.FileNotFound, "file_not_found");
        }
    }

    private async Task ValidateCategoryExistsIfSetAsync(Guid? productCategoryId, CancellationToken ct)
    {
        if (!productCategoryId.HasValue)
            return;

        var cat = await _categoryRepository.GetByIdAsync(productCategoryId.Value);
        if (cat is null || cat.IsDeleted)
            throw new BadRequestException(MessageKeys.ProductCategoryNotFound, "product_category_not_found");
    }

    private static void ValidateAttributeType(ProductAttributeType type)
    {
        if (type is not (ProductAttributeType.Selectable or ProductAttributeType.Text))
            throw new BadRequestException(MessageKeys.ProductAttributeTypeInvalid, "product_attribute_type_invalid");
    }

    private async Task EnsureAttributeExistsAsync(Guid attributeId, CancellationToken ct)
    {
        var prop = await _attributeRepository.GetByIdAsync(attributeId);
        if (prop is null || prop.IsDeleted)
            throw new BadRequestException(MessageKeys.ProductAttributeNotFound, "product_attribute_not_found");
    }

    private async Task EnsureProductExistsAsync(Guid productId, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product is null || product.IsDeleted)
            throw new NotFoundException(MessageKeys.ProductNotFound, "product_not_found");
    }

    private static void ValidateProductAttributeValue(string? value)
    {
        if (value is not null && value.Length > 100)
            throw new BadRequestException(MessageKeys.ProductProductAttributeValueMaxLength, "product_product_attribute_value_max_length");
    }

    private static ProductCategoryDto MapCategoryFlat(ProductCategory c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        IsActive = c.IsActive,
        ParentCategoryId = c.ParentCategoryId,
        ParentCategoryName = c.ParentCategory?.Name,
        ImageId = c.ImageId,
        ImagePath = c.Image?.Path,
        CreatedAt = c.CreatedAt
    };

    private static ProductAttributeDto MapAttribute(ProductAttribute p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Type = p.Type,
        SortOrder = p.SortOrder,
        ProductCategoryId = p.ProductCategoryId,
        ProductCategoryName = p.ProductCategory?.Name,
        CreatedAt = p.CreatedAt
    };

    private static ProductAttributeValueDto MapAttributeValue(ProductAttributeValue v) => new()
    {
        Id = v.Id,
        Title = v.Title,
        Value = v.Value,
        ProductAttributeId = v.ProductAttributeId,
        ProductAttributeName = v.ProductAttribute?.Name,
        IsActive = v.IsActive,
        SortOrder = v.SortOrder,
        CreatedAt = v.CreatedAt
    };

    private static ProductProductAttributeDto MapProductAttribute(ProductProductAttribute x) => new()
    {
        Id = x.Id,
        ProductId = x.ProductId,
        ProductName = x.Product?.Name,
        ProductAttributeId = x.ProductAttributeId,
        ProductAttributeName = x.ProductAttribute?.Name,
        Value = x.Value,
        CreatedAt = x.CreatedAt
    };
}