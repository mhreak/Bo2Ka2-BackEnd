using Bodokado.Application.App.AdminModule.ProductCatalog.DTOs;
using Bodokado.Application.App.AdminModule.ProductCatalog.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.File.Interfaces;
using Bodokado.Application.Common.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Entities.Products;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.AdminModule.ProductCatalog.Services;

public class ProductCatalogService : IProductCatalogService
{
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly IProductPropertyRepository _propertyRepository;
    private readonly IFileAssetRepository _fileAssetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductCatalogService(
        IProductCategoryRepository categoryRepository,
        IProductPropertyRepository propertyRepository,
        IFileAssetRepository fileAssetRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _propertyRepository = propertyRepository;
        _fileAssetRepository = fileAssetRepository;
        _unitOfWork = unitOfWork;
    }

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

    public async Task<List<ProductPropertyDto>> GetPropertiesAsync(Guid? productCategoryId = null, CancellationToken ct = default)
    {
        var list = await _propertyRepository.GetListAsync(productCategoryId, ct);
        return list.Select(MapProperty).ToList();
    }

    public async Task<ProductPropertyDto> GetPropertyByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _propertyRepository.GetByIdWithCategoryAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductPropertyNotFound, "product_property_not_found");
        return MapProperty(entity);
    }

    public async Task<ProductPropertyDto> CreatePropertyAsync(CreateProductPropertyRequestDto request, CancellationToken ct = default)
    {
        await ValidateCategoryExistsIfSetAsync(request.ProductCategoryId, ct);
        ValidatePropertyType(request.Type);

        var entity = new ProductProperty
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Type = request.Type,
            SortOrder = request.SortOrder,
            ProductCategoryId = request.ProductCategoryId,
            CreatedAt = DateTime.UtcNow
        };

        await _propertyRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        var created = await _propertyRepository.GetByIdWithCategoryAsync(entity.Id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductPropertyNotFound, "product_property_not_found");
        return MapProperty(created);
    }

    public async Task<ProductPropertyDto> UpdatePropertyAsync(Guid id, UpdateProductPropertyRequestDto request, CancellationToken ct = default)
    {
        var entity = await _propertyRepository.GetByIdWithCategoryAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductPropertyNotFound, "product_property_not_found");

        await ValidateCategoryExistsIfSetAsync(request.ProductCategoryId, ct);
        ValidatePropertyType(request.Type);

        entity.Name = request.Name.Trim();
        entity.Type = request.Type;
        entity.SortOrder = request.SortOrder;
        entity.ProductCategoryId = request.ProductCategoryId;
        entity.UpdatedAt = DateTime.UtcNow;

        _propertyRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _propertyRepository.GetByIdWithCategoryAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductPropertyNotFound, "product_property_not_found");
        return MapProperty(updated);
    }

    public async Task DeletePropertyAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _propertyRepository.GetByIdWithCategoryAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductPropertyNotFound, "product_property_not_found");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _propertyRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }

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

    private static void ValidatePropertyType(ProductPropertyType type)
    {
        if (type is not (ProductPropertyType.Selectable or ProductPropertyType.Text))
            throw new BadRequestException(MessageKeys.ProductPropertyTypeInvalid, "product_property_type_invalid");
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

    private static ProductPropertyDto MapProperty(ProductProperty p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Type = p.Type,
        SortOrder = p.SortOrder,
        ProductCategoryId = p.ProductCategoryId,
        ProductCategoryName = p.ProductCategory?.Name,
        CreatedAt = p.CreatedAt
    };
}