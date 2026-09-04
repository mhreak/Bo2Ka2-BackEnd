using Bodokado.Application.App.ShopModule.Orders.DTOs;
using Bodokado.Application.App.ShopModule.Orders.Interfaces;
using Bodokado.Application.App.ShopModule.Registration.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.File.Interfaces;
using Bodokado.Application.Common.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Entities;
using Bodokado.Domain.Entities.Orders;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.ShopModule.Orders.Services;

public class ShopOrderService : IShopOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IShopRepository _shopRepository;
    private readonly IFileAssetRepository _fileAssetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ShopOrderService(
        IOrderRepository orderRepository,
        IShopRepository shopRepository,
        IFileAssetRepository fileAssetRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _shopRepository = shopRepository;
        _fileAssetRepository = fileAssetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<OrderListItemDto>> GetOrdersAsync(Guid shopUserId, OrderListQuery query, CancellationToken ct = default)
    {
        var shop = await GetApprovedShopAsync(shopUserId, ct);
        var paged = await _orderRepository.GetPagedForShopAsync(shop.Id, query, ct);
        var fileMap = await LoadPrimaryImageMapAsync(paged.Items);
        var items = paged.Items.Select(o => MapListItem(o, fileMap)).ToList();
        return PagedResult<OrderListItemDto>.Create(items, query, paged.TotalCount);
    }

    public async Task<OrderDetailDto> GetByIdAsync(Guid shopUserId, Guid orderId, CancellationToken ct = default)
    {
        var shop = await GetApprovedShopAsync(shopUserId, ct);
        var order = await _orderRepository.GetByIdWithDetailsForShopAsync(orderId, shop.Id, ct)
            ?? throw new NotFoundException(MessageKeys.OrderNotFound, "order_not_found");
        var fileMap = await LoadItemImageMapAsync(order);
        return MapDetail(order, fileMap);
    }

    public async Task<OrderDetailDto> ConfirmAsync(Guid shopUserId, Guid orderId, ConfirmOrderRequestDto request, CancellationToken ct = default)
    {
        var shop = await GetApprovedShopAsync(shopUserId, ct);
        var order = await _orderRepository.GetByIdWithDetailsForShopAsync(orderId, shop.Id, ct)
            ?? throw new NotFoundException(MessageKeys.OrderNotFound, "order_not_found");

        if (order.Status != OrderStatus.Pending)
            throw new BadRequestException(MessageKeys.OrderInvalidStatus, "order_invalid_status");

        if (!string.IsNullOrWhiteSpace(request.PackagingType))
            order.PackagingType = request.PackagingType.Trim();
        if (request.PackagingNote is not null)
            order.PackagingNote = string.IsNullOrWhiteSpace(request.PackagingNote) ? null : request.PackagingNote.Trim();
        if (request.HasSpecialPackaging.HasValue)
            order.HasSpecialPackaging = request.HasSpecialPackaging.Value;
        if (request.PackagingCost.HasValue)
            order.PackagingCost = Math.Max(0, request.PackagingCost.Value);
        if (request.ShippingCost.HasValue)
            order.ShippingCost = Math.Max(0, request.ShippingCost.Value);
        if (request.ApplyDiscountCode.HasValue)
            order.ApplyDiscountCode = request.ApplyDiscountCode.Value;
        if (request.ShopNote is not null)
            order.ShopNote = string.IsNullOrWhiteSpace(request.ShopNote) ? null : request.ShopNote.Trim();

        RecalculateFinalAmount(order);

        order.Status = OrderStatus.Confirmed;
        order.ConfirmedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(ct);

        var fileMap = await LoadItemImageMapAsync(order);
        return MapDetail(order, fileMap);
    }

    public async Task<OrderDetailDto> RejectAsync(Guid shopUserId, Guid orderId, RejectOrderRequestDto request, CancellationToken ct = default)
    {
        var shop = await GetApprovedShopAsync(shopUserId, ct);
        var order = await _orderRepository.GetByIdWithDetailsForShopAsync(orderId, shop.Id, ct)
            ?? throw new NotFoundException(MessageKeys.OrderNotFound, "order_not_found");

        if (order.Status != OrderStatus.Pending)
            throw new BadRequestException(MessageKeys.OrderInvalidStatus, "order_invalid_status");

        order.Status = OrderStatus.Rejected;
        order.RejectionReason = string.IsNullOrWhiteSpace(request.Reason) ? null : request.Reason.Trim();
        order.RejectedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;

        // بازگرداندن موجودی
        foreach (var item in order.Items.Where(i => !i.IsDeleted))
        {
            if (item.Product is not null)
            {
                item.Product.StockQuantity += item.Quantity;
                if (item.Product.SoldCount >= item.Quantity)
                    item.Product.SoldCount -= item.Quantity;
            }
        }

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(ct);

        var fileMap = await LoadItemImageMapAsync(order);
        return MapDetail(order, fileMap);
    }

    public async Task<OrderDetailDto> MarkShippedAsync(Guid shopUserId, Guid orderId, CancellationToken ct = default)
    {
        var shop = await GetApprovedShopAsync(shopUserId, ct);
        var order = await _orderRepository.GetByIdWithDetailsForShopAsync(orderId, shop.Id, ct)
            ?? throw new NotFoundException(MessageKeys.OrderNotFound, "order_not_found");

        if (order.Status != OrderStatus.Confirmed)
            throw new BadRequestException(MessageKeys.OrderInvalidStatus, "order_invalid_status");

        order.Status = OrderStatus.Shipped;
        order.ShippedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(ct);

        var fileMap = await LoadItemImageMapAsync(order);
        return MapDetail(order, fileMap);
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

    private static void RecalculateFinalAmount(Order order)
    {
        order.FinalAmount = order.GoodsAmount + order.ShippingCost + order.PackagingCost - order.DiscountAmount;
        if (order.FinalAmount < 0) order.FinalAmount = 0;
    }

    private async Task<Dictionary<Guid, FileAsset>> LoadPrimaryImageMapAsync(IEnumerable<Order> orders)
    {
        var ids = orders
            .SelectMany(o => o.Items.Where(i => !i.IsDeleted && i.ProductImageFileId.HasValue).Select(i => i.ProductImageFileId!.Value))
            .Distinct()
            .ToList();
        return await LoadFilesAsync(ids);
    }

    private async Task<Dictionary<Guid, FileAsset>> LoadItemImageMapAsync(Order order)
    {
        var ids = order.Items
            .Where(i => !i.IsDeleted && i.ProductImageFileId.HasValue)
            .Select(i => i.ProductImageFileId!.Value)
            .Distinct()
            .ToList();
        return await LoadFilesAsync(ids);
    }

    private async Task<Dictionary<Guid, FileAsset>> LoadFilesAsync(List<Guid> ids)
    {
        var map = new Dictionary<Guid, FileAsset>();
        foreach (var id in ids)
        {
            var file = await _fileAssetRepository.GetByIdAsync(id);
            if (file is not null && !file.IsDeleted)
                map[id] = file;
        }
        return map;
    }

    internal static OrderListItemDto MapListItem(Order o, Dictionary<Guid, FileAsset> fileMap)
    {
        var first = o.Items.Where(i => !i.IsDeleted).OrderBy(i => i.CreatedAt).FirstOrDefault();
        string? imagePath = null;
        if (first?.ProductImageFileId is Guid fid && fileMap.TryGetValue(fid, out var f))
            imagePath = f.Path;

        return new OrderListItemDto
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            ProductName = first?.ProductName ?? string.Empty,
            ProductImagePath = imagePath,
            FinalAmount = o.FinalAmount,
            BuyerName = o.BuyerName,
            CreatedAt = o.CreatedAt,
            Status = o.Status,
            ItemsCount = o.Items.Count(i => !i.IsDeleted)
        };
    }

    internal static OrderDetailDto MapDetail(Order o, Dictionary<Guid, FileAsset> fileMap)
    {
        return new OrderDetailDto
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            Status = o.Status,
            Items = o.Items.Where(i => !i.IsDeleted).Select(i =>
            {
                string? path = null;
                if (i.ProductImageFileId is Guid fid && fileMap.TryGetValue(fid, out var f))
                    path = f.Path;
                return new OrderItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductImageFileId = i.ProductImageFileId,
                    ProductImagePath = path,
                    SelectedColor = i.SelectedColor,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    LineTotal = i.LineTotal
                };
            }).ToList(),
            ShippingMethod = o.ShippingMethod,
            CreatedAt = o.CreatedAt,
            DeliveryDate = o.DeliveryDate,
            DeliveryTimeSlot = o.DeliveryTimeSlot,
            PackagingType = o.PackagingType,
            PackagingNote = o.PackagingNote,
            HasSpecialPackaging = o.HasSpecialPackaging,
            BuyerName = o.BuyerName,
            BuyerPhone = o.BuyerPhone,
            DeliveryAddress = o.DeliveryAddress,
            ProvinceId = o.ProvinceId,
            CityId = o.CityId,
            Latitude = o.Latitude,
            Longitude = o.Longitude,
            GiftCardType = o.GiftCardType,
            GiftCardColor = o.GiftCardColor,
            RibbonStyle = o.RibbonStyle,
            GiftCardDesignKey = o.GiftCardDesignKey,
            GiftMessage = o.GiftMessage,
            RecipientName = o.RecipientName,
            GoodsAmount = o.GoodsAmount,
            ShippingCost = o.ShippingCost,
            PackagingCost = o.PackagingCost,
            DiscountAmount = o.DiscountAmount,
            ApplyDiscountCode = o.ApplyDiscountCode,
            FinalAmount = o.FinalAmount,
            PaymentMethod = o.PaymentMethod,
            PaymentStatus = o.PaymentStatus,
            RejectionReason = o.RejectionReason,
            ShopNote = o.ShopNote,
            ConfirmedAt = o.ConfirmedAt,
            ShippedAt = o.ShippedAt
        };
    }
}
