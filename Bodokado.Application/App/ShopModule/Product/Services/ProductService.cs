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

    public async Task<PagedResult<ProductListItemDto>> GetMyProductsAsync(Guid userId, ProductListQuery query, CancellationToken ct = default)
    {
        var shop = await GetApprovedShopAsync(userId, ct);
        var paged = await _productRepository.GetPagedForShopAsync(shop.Id, query, ct);

        var allFileIds = paged.Items
            .SelectMany(p => p.ImageFileIds ?? Enumerable.Empty<Guid>())
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

        var fileMap = await LoadFileMapAsync(product.ImageFileIds);
        return MapDetail(product, fileMap);
    }

    public async Task<ProductDetailDto> CreateAsync(Guid userId, CreateProductRequestDto request, CancellationToken ct = default)
    {
        var shop = await GetApprovedShopAsync(userId, ct);
        ValidatePricing(request.IsDiscountEnabled, request.BasePrice, request.DiscountPrice);

        var imageFileIds = await ValidateAndNormalizeImageIdsAsync(request.ImageFileIds, userId);

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
            ImageFileIds = imageFileIds,
            CreatedAt = DateTime.UtcNow
        };

        ApplyColors(product, request.Colors);

        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync(ct);

        var created = await _productRepository.GetByIdWithDetailsForShopAsync(product.Id, shop.Id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductNotFound, "product_not_found");

        var fileMap = await LoadFileMapAsync(created.ImageFileIds);
        return MapDetail(created, fileMap);
    }

    public async Task<ProductDetailDto> UpdateAsync(Guid userId, Guid productId, UpdateProductRequestDto request, CancellationToken ct = default)
    {
        var shop = await GetApprovedShopAsync(userId, ct);
        var product = await _productRepository.GetByIdWithDetailsForShopAsync(productId, shop.Id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductNotFound, "product_not_found");

        ValidatePricing(request.IsDiscountEnabled, request.BasePrice, request.DiscountPrice);

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
        product.ImageFileIds = await ValidateAndNormalizeImageIdsAsync(request.ImageFileIds, userId);
        product.UpdatedAt = DateTime.UtcNow;

        foreach (var color in product.Colors.Where(c => !c.IsDeleted).ToList())
        {
            color.IsDeleted = true;
            color.UpdatedAt = DateTime.UtcNow;
        }
        ApplyColors(product, request.Colors);

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(ct);

        var updated = await _productRepository.GetByIdWithDetailsForShopAsync(product.Id, shop.Id, ct)
            ?? throw new NotFoundException(MessageKeys.ProductNotFound, "product_not_found");

        var fileMap = await LoadFileMapAsync(updated.ImageFileIds);
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

    /// <summary>
    /// شناسه‌های فایل از API عمومی آپلود را اعتبارسنجی می‌کند (وجود + مالکیت کاربر).
    /// </summary>
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

    private static void ApplyColors(Product product, List<ProductColorDto>? colors)
    {
        if (colors is null || colors.Count == 0)
            return;

        var order = 0;
        foreach (var c in colors)
        {
            if (string.IsNullOrWhiteSpace(c.Name))
                continue;

            product.Colors.Add(new ProductColor
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Name = c.Name.Trim(),
                HexCode = string.IsNullOrWhiteSpace(c.HexCode) ? null : c.HexCode.Trim(),
                SortOrder = c.SortOrder > 0 ? c.SortOrder : order,
                CreatedAt = DateTime.UtcNow
            });
            order++;
        }
    }

    private static decimal GetEffectivePrice(Product p)
        => p.IsDiscountEnabled && p.DiscountPrice.HasValue ? p.DiscountPrice.Value : p.BasePrice;

    private static ProductListItemDto MapListItem(Product p, Dictionary<Guid, FileAsset> fileMap)
    {
        string? primaryPath = null;
        if (p.ImageFileIds is { Count: > 0 } && fileMap.TryGetValue(p.ImageFileIds[0], out var primaryFile))
            primaryPath = primaryFile.Path;

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
            PrimaryImagePath = primaryPath,
            CreatedAt = p.CreatedAt
        };
    }

    private static ProductDetailDto MapDetail(Product p, Dictionary<Guid, FileAsset> fileMap)
    {
        var images = new List<ProductImageDto>();
        var order = 0;
        foreach (var fileId in p.ImageFileIds ?? Enumerable.Empty<Guid>())
        {
            fileMap.TryGetValue(fileId, out var file);
            images.Add(new ProductImageDto
            {
                FileAssetId = fileId,
                Path = file?.Path,
                SortOrder = order,
                IsPrimary = order == 0
            });
            order++;
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
            Images = images,
            Colors = p.Colors
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.SortOrder)
                .Select(c => new ProductColorDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    HexCode = c.HexCode,
                    SortOrder = c.SortOrder
                }).ToList(),
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };
    }
}
