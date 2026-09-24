using Bodokado.Application.App.ShopModule.Products.DTOs;
using Bodokado.Application.App.ShopModule.Products.Interfaces;
using Bodokado.Application.App.ShopModule.Registration.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.File.Interfaces;
using Bodokado.Application.Common.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Entities;
using Bodokado.Domain.Entities.Products;
using Bodokado.Domain.Entities.Shops;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.ShopModule.Products.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IShopRepository _shopRepository;
    private readonly IFileAssetRepository _fileAssetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(
        IProductRepository productRepository,
        IShopRepository shopRepository,
        IFileAssetRepository fileAssetRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _shopRepository = shopRepository;
        _fileAssetRepository = fileAssetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<ProductListItemDto>> GetMyProductsAsync(
        Guid userId, ProductListQuery query, CancellationToken ct = default)
    {
        var shop = await GetApprovedShopAsync(userId, ct);
        var paged = await _productRepository.GetPagedForShopAsync(shop.Id, query, ct);

        var allFileIds = paged.Items
            .SelectMany(p => CollectImageFileIds(p))
            .Distinct()
            .ToList();

        var fileMap = await LoadFileMapAsync(allFileIds);
        var items = paged.Items.Select(p => MapListItem(p, fileMap)).ToList();
        return PagedResult<ProductListItemDto>.Create(items, query, paged.TotalCount);
    }

    public async Task<ProductDetailDto> GetByIdAsync(Guid userId, Guid productId, CancellationToken ct = default)
    {
        var shop = await GetApprovedShopAsync(userId, ct);
        var product = await _productRepository.GetByIdWithDetailsForShopAsync(productId, shop.Id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductNotFound, "product_not_found");

        var fileMap = await LoadFileMapAsync(CollectImageFileIds(product));
        return MapDetail(product, fileMap);
    }

    public async Task<ProductDetailDto> CreateAsync(
        Guid userId, CreateProductRequestDto request, CancellationToken ct = default)
    {
        var shop = await GetApprovedShopAsync(userId, ct);
        ValidatePricing(request.IsDiscountEnabled, request.BasePrice, request.DiscountPrice);

        var mainImageId = await ValidateFileOwnedByUserAsync(request.MainImageFileId, userId);
        var extraImageIds = await ValidateAndNormalizeImageIdsAsync(request.ImageFileIds, userId);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            ShopId = shop.Id,
            Name = request.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            WeightGrams = request.WeightGrams,
            LengthCm = request.LengthCm,
            WidthCm = request.WidthCm,
            HeightCm = request.HeightCm,
            Brand = string.IsNullOrWhiteSpace(request.Brand) ? null : request.Brand.Trim(),
            BasePrice = request.BasePrice,
            IsDiscountEnabled = request.IsDiscountEnabled,
            DiscountPrice = request.IsDiscountEnabled ? request.DiscountPrice : null,
            StockQuantity = request.StockQuantity,
            HasSpecialPackaging = request.HasSpecialPackaging,
            IsSpecial = request.IsSpecial,
            Status = request.Publish ? ProductStatus.Published : ProductStatus.Draft,
            ProductType = request.ProductType,
            MainImageFileId = mainImageId,
            IsActiveByAdmin = true, // فقط ادمین عوض می‌کند
            CreatedAt = DateTime.UtcNow
        };

        // تصاویر فرعی
        var sort = 0;
        foreach (var fileId in extraImageIds)
        {
            product.Images.Add(new ShopProductImage
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                FileId = fileId,
                SortOrder = sort++,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync(ct);

        var created = await _productRepository.GetByIdWithDetailsForShopAsync(product.Id, shop.Id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductNotFound, "product_not_found");

        var fileMap = await LoadFileMapAsync(CollectImageFileIds(created));
        return MapDetail(created, fileMap);
    }

    public async Task<ProductDetailDto> UpdateAsync(
        Guid userId, Guid productId, UpdateProductRequestDto request, CancellationToken ct = default)
    {
        var shop = await GetApprovedShopAsync(userId, ct);
        var product = await _productRepository.GetByIdWithDetailsForShopAsync(productId, shop.Id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductNotFound, "product_not_found");

        ValidatePricing(request.IsDiscountEnabled, request.BasePrice, request.DiscountPrice);

        var mainImageId = await ValidateFileOwnedByUserAsync(request.MainImageFileId, userId);
        var extraImageIds = await ValidateAndNormalizeImageIdsAsync(request.ImageFileIds, userId);

        product.Name = request.Name.Trim();
        product.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        product.WeightGrams = request.WeightGrams;
        product.LengthCm = request.LengthCm;
        product.WidthCm = request.WidthCm;
        product.HeightCm = request.HeightCm;
        product.Brand = string.IsNullOrWhiteSpace(request.Brand) ? null : request.Brand.Trim();
        product.BasePrice = request.BasePrice;
        product.IsDiscountEnabled = request.IsDiscountEnabled;
        product.DiscountPrice = request.IsDiscountEnabled ? request.DiscountPrice : null;
        product.StockQuantity = request.StockQuantity;
        product.HasSpecialPackaging = request.HasSpecialPackaging;
        product.IsSpecial = request.IsSpecial;
        product.Status = request.Publish ? ProductStatus.Published : ProductStatus.Draft;
        product.ProductType = request.ProductType;
        product.MainImageFileId = mainImageId;
        product.UpdatedAt = DateTime.UtcNow;
        // IsActiveByAdmin را از request نخوان

        // جایگزینی تصاویر فرعی
        foreach (var old in product.Images.ToList())
        {
            old.IsDeleted = true;
            old.UpdatedAt = DateTime.UtcNow;
        }

        var sort = 0;
        foreach (var fileId in extraImageIds)
        {
            product.Images.Add(new ShopProductImage
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                FileId = fileId,
                SortOrder = sort++,
                CreatedAt = DateTime.UtcNow
            });
        }

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _productRepository.GetByIdWithDetailsForShopAsync(product.Id, shop.Id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductNotFound, "product_not_found");

        var fileMap = await LoadFileMapAsync(CollectImageFileIds(updated));
        return MapDetail(updated, fileMap);
    }

    public async Task DeleteAsync(Guid userId, Guid productId, CancellationToken ct = default)
    {
        var shop = await GetApprovedShopAsync(userId, ct);
        var product = await _productRepository.GetByIdForShopAsync(productId, shop.Id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductNotFound, "product_not_found");

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    // ───────────── Helpers ─────────────

    private async Task<Domain.Entities.Shops.Shop> GetApprovedShopAsync(Guid userId, CancellationToken ct)
    {
        var shop = await _shopRepository.GetByUserIdAsync(userId, ct);
        if (shop is null)
            throw new BadRequestException(MessageKeys.ShopNotFound, "shop_not_found");
        if (shop.VerificationStatus != ShopVerificationStatus.Approved)
            throw new BadRequestException(MessageKeys.ShopNotApproved, "shop_not_approved");
        return shop;
    }

    private static void ValidatePricing(bool isDiscountEnabled, decimal basePrice, decimal? discountPrice)
    {
        if (basePrice <= 0)
            throw new BadRequestException(MessageKeys.ProductBasePriceInvalid, "product_base_price_invalid");

        if (isDiscountEnabled)
        {
            if (discountPrice is null || discountPrice <= 0)
                throw new BadRequestException(MessageKeys.ProductDiscountPriceRequired, "product_discount_price_required");
            if (discountPrice >= basePrice)
                throw new BadRequestException(MessageKeys.ProductDiscountPriceInvalid, "product_discount_price_invalid");
        }
    }

    private async Task<Guid?> ValidateFileOwnedByUserAsync(Guid? fileId, Guid userId)
    {
        if (!fileId.HasValue)
            return null;

        var file = await _fileAssetRepository.GetByIdAsync(fileId.Value);
        if (file is null || file.IsDeleted)
            throw new BadRequestException(MessageKeys.FileNotFound, "file_not_found");
        if (file.UploaderId != userId)
            throw new BadRequestException(MessageKeys.FileNotOwnedByUser, "file_not_owned");

        return fileId;
    }

    private async Task<List<Guid>> ValidateAndNormalizeImageIdsAsync(List<Guid>? imageFileIds, Guid userId)
    {
        if (imageFileIds is null || imageFileIds.Count == 0)
            return new List<Guid>();

        var distinctIds = imageFileIds.Distinct().ToList();
        if (distinctIds.Count > 10)
            throw new BadRequestException(MessageKeys.ProductImagesMaxCount, "product_images_max_count");

        var result = new List<Guid>(distinctIds.Count);
        foreach (var fileId in distinctIds)
        {
            var file = await _fileAssetRepository.GetByIdAsync(fileId);
            if (file is null || file.IsDeleted)
                throw new BadRequestException(MessageKeys.FileNotFound, "file_not_found");
            if (file.UploaderId != userId)
                throw new BadRequestException(MessageKeys.FileNotOwnedByUser, "file_not_owned");
            result.Add(fileId);
        }
        return result;
    }

    private static IEnumerable<Guid> CollectImageFileIds(Product p)
    {
        if (p.MainImageFileId.HasValue)
            yield return p.MainImageFileId.Value;

        if (p.Images is null)
            yield break;

        foreach (var img in p.Images.Where(i => !i.IsDeleted))
            yield return img.FileId;
    }

    private async Task<Dictionary<Guid, FileAsset>> LoadFileMapAsync(IEnumerable<Guid>? ids)
    {
        var map = new Dictionary<Guid, FileAsset>();
        if (ids is null)
            return map;

        foreach (var id in ids.Distinct())
        {
            var file = await _fileAssetRepository.GetByIdAsync(id);
            if (file is not null && !file.IsDeleted)
                map[id] = file;
        }
        return map;
    }

    private static decimal GetEffectivePrice(Product p)
        => p.IsDiscountEnabled && p.DiscountPrice.HasValue ? p.DiscountPrice.Value : p.BasePrice;

    private static ProductListItemDto MapListItem(Product p, Dictionary<Guid, FileAsset> fileMap)
    {
        string? primaryPath = null;
        if (p.MainImageFileId.HasValue && fileMap.TryGetValue(p.MainImageFileId.Value, out var mainFile))
            primaryPath = mainFile.Path;
        else if (p.Images is { Count: > 0 })
        {
            var first = p.Images.Where(i => !i.IsDeleted).OrderBy(i => i.SortOrder).FirstOrDefault();
            if (first is not null && fileMap.TryGetValue(first.FileId, out var f))
                primaryPath = f.Path;
        }

        return new ProductListItemDto
        {
            Id = p.Id,
            Name = p.Name,
            BasePrice = p.BasePrice,
            IsDiscountEnabled = p.IsDiscountEnabled,
            DiscountPrice = p.DiscountPrice,
            EffectivePrice = GetEffectivePrice(p),
            StockQuantity = p.StockQuantity,
            IsInStock = p.StockQuantity > 0,
            IsSpecial = p.IsSpecial,
            SoldCount = p.SoldCount,
            Status = p.Status,
            ProductType = p.ProductType,
            PrimaryImagePath = primaryPath,
            CreatedAt = p.CreatedAt
        };
    }

    private static ProductDetailDto MapDetail(Product p, Dictionary<Guid, FileAsset> fileMap)
    {
        var images = new List<ProductImageDto>();

        if (p.MainImageFileId.HasValue)
        {
            fileMap.TryGetValue(p.MainImageFileId.Value, out var main);
            images.Add(new ProductImageDto
            {
                FileAssetId = p.MainImageFileId.Value,
                Path = main?.Path,
                SortOrder = -1,
                IsPrimary = true
            });
        }

        foreach (var img in (p.Images ?? new List<ShopProductImage>())
                     .Where(i => !i.IsDeleted)
                     .OrderBy(i => i.SortOrder))
        {
            fileMap.TryGetValue(img.FileId, out var file);
            images.Add(new ProductImageDto
            {
                FileAssetId = img.FileId,
                Path = file?.Path,
                SortOrder = img.SortOrder,
                IsPrimary = false
            });
        }

        return new ProductDetailDto
        {
            Id = p.Id,
            ShopId = p.ShopId,
            Name = p.Name,
            Description = p.Description,
            WeightGrams = p.WeightGrams,
            LengthCm = p.LengthCm,
            WidthCm = p.WidthCm,
            HeightCm = p.HeightCm,
            Brand = p.Brand,
            BasePrice = p.BasePrice,
            IsDiscountEnabled = p.IsDiscountEnabled,
            DiscountPrice = p.DiscountPrice,
            EffectivePrice = GetEffectivePrice(p),
            StockQuantity = p.StockQuantity,
            IsInStock = p.StockQuantity > 0,
            HasSpecialPackaging = p.HasSpecialPackaging,
            IsSpecial = p.IsSpecial,
            SoldCount = p.SoldCount,
            Status = p.Status,
            ProductType = p.ProductType,
            MainImageFileId = p.MainImageFileId,
            Images = images,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };
    }
}