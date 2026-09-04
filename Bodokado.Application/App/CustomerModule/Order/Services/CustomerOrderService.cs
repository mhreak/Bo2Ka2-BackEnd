using Bodokado.Application.App.CustomerModule.Orders.DTOs;
using Bodokado.Application.App.CustomerModule.Orders.Interfaces;
using Bodokado.Application.App.ShopModule.Orders.DTOs;
using Bodokado.Application.App.ShopModule.Orders.Interfaces;
using Bodokado.Application.App.ShopModule.Orders.Services;
using Bodokado.Application.App.ShopModule.Products.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.File.Interfaces;
using Bodokado.Application.Common.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Entities;
using Bodokado.Domain.Entities.Orders;
using Bodokado.Domain.Entities.Products;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.CustomerModule.Orders.Services;

public class CustomerOrderService : ICustomerOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IFileAssetRepository _fileAssetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerOrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IFileAssetRepository fileAssetRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _fileAssetRepository = fileAssetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderDetailDto> CreateAsync(Guid customerUserId, CreateOrderRequestDto request, CancellationToken ct = default)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new BadRequestException(MessageKeys.OrderItemsRequired, "order_items_required");

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = new List<Product>();
        foreach (var pid in productIds)
        {
            var p = await _productRepository.GetByIdAsync(pid);
            if (p is null || p.IsDeleted || p.Status != ProductStatus.Published)
                throw new BadRequestException(MessageKeys.ProductNotFound, "product_not_found");
            products.Add(p);
        }

        var shopIds = products.Select(p => p.ShopId).Distinct().ToList();
        if (shopIds.Count > 1)
            throw new BadRequestException(MessageKeys.OrderItemsMustSameShop, "order_items_must_same_shop");

        var shopId = shopIds[0];
        var productMap = products.ToDictionary(p => p.Id);

        decimal goodsAmount = 0;
        var orderItems = new List<OrderItem>();

        foreach (var reqItem in request.Items)
        {
            if (reqItem.Quantity <= 0)
                throw new BadRequestException(MessageKeys.OrderQuantityInvalid, "order_quantity_invalid");

            var product = productMap[reqItem.ProductId];
            if (product.StockQuantity < reqItem.Quantity)
                throw new BadRequestException(MessageKeys.OrderInsufficientStock, "order_insufficient_stock");

            var unitPrice = product.IsDiscountEnabled && product.DiscountPrice.HasValue
                ? product.DiscountPrice.Value
                : product.BasePrice;

            var lineTotal = unitPrice * reqItem.Quantity;
            goodsAmount += lineTotal;

            orderItems.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                ProductName = product.Name,
                ProductImageFileId = product.ImageFileIds?.FirstOrDefault(),
                SelectedColor = string.IsNullOrWhiteSpace(reqItem.SelectedColor) ? null : reqItem.SelectedColor.Trim(),
                UnitPrice = unitPrice,
                Quantity = reqItem.Quantity,
                LineTotal = lineTotal,
                CreatedAt = DateTime.UtcNow
            });

            product.StockQuantity -= reqItem.Quantity;
            product.SoldCount += reqItem.Quantity;
            _productRepository.Update(product);
        }

        // هزینه‌های پیش‌فرض (فروشگاه می‌تواند در تایید تغییر دهد)
        decimal shippingCost = request.ShippingMethod == ShippingMethod.Express ? 50000 : 30000;
        decimal packagingCost = request.HasSpecialPackaging ? 25000 : 0;
        decimal discountAmount = 0;

        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = await _orderRepository.GenerateOrderNumberAsync(ct),
            ShopId = shopId,
            CustomerUserId = customerUserId,
            Status = OrderStatus.Pending,
            BuyerName = request.BuyerName.Trim(),
            BuyerPhone = request.BuyerPhone.Trim(),
            DeliveryAddress = string.IsNullOrWhiteSpace(request.DeliveryAddress) ? null : request.DeliveryAddress.Trim(),
            ProvinceId = request.ProvinceId,
            CityId = request.CityId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            ShippingMethod = request.ShippingMethod,
            DeliveryDate = request.DeliveryDate,
            DeliveryTimeSlot = string.IsNullOrWhiteSpace(request.DeliveryTimeSlot) ? null : request.DeliveryTimeSlot.Trim(),
            PackagingType = string.IsNullOrWhiteSpace(request.PackagingType) ? null : request.PackagingType.Trim(),
            PackagingNote = string.IsNullOrWhiteSpace(request.PackagingNote) ? null : request.PackagingNote.Trim(),
            HasSpecialPackaging = request.HasSpecialPackaging,
            GiftCardType = NullIfWhite(request.GiftCardType),
            GiftCardColor = NullIfWhite(request.GiftCardColor),
            RibbonStyle = NullIfWhite(request.RibbonStyle),
            GiftCardDesignKey = NullIfWhite(request.GiftCardDesignKey),
            GiftMessage = NullIfWhite(request.GiftMessage),
            RecipientName = NullIfWhite(request.RecipientName),
            GoodsAmount = goodsAmount,
            ShippingCost = shippingCost,
            PackagingCost = packagingCost,
            DiscountAmount = discountAmount,
            ApplyDiscountCode = !string.IsNullOrWhiteSpace(request.DiscountCode),
            DiscountCode = NullIfWhite(request.DiscountCode),
            FinalAmount = goodsAmount + shippingCost + packagingCost - discountAmount,
            PaymentMethod = request.PaymentMethod,
            PaymentStatus = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Items = orderItems
        };

        foreach (var item in orderItems)
            item.OrderId = order.Id;

        await _orderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync(ct);

        var created = await _orderRepository.GetByIdWithDetailsForCustomerAsync(order.Id, customerUserId, ct)
            ?? throw new NotFoundException(MessageKeys.OrderNotFound, "order_not_found");

        var fileMap = await LoadItemImagesAsync(created);
        return ShopOrderService.MapDetail(created, fileMap);
    }

    public async Task<PagedResult<OrderListItemDto>> GetMyOrdersAsync(Guid customerUserId, PaginationQuery query, CancellationToken ct = default)
    {
        var paged = await _orderRepository.GetPagedForCustomerAsync(customerUserId, query, ct);
        var fileMap = await LoadPrimaryImagesAsync(paged.Items);
        var items = paged.Items.Select(o => ShopOrderService.MapListItem(o, fileMap)).ToList();
        return PagedResult<OrderListItemDto>.Create(items, query, paged.TotalCount);
    }

    public async Task<OrderDetailDto> GetByIdAsync(Guid customerUserId, Guid orderId, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdWithDetailsForCustomerAsync(orderId, customerUserId, ct)
            ?? throw new NotFoundException(MessageKeys.OrderNotFound, "order_not_found");
        var fileMap = await LoadItemImagesAsync(order);
        return ShopOrderService.MapDetail(order, fileMap);
    }

    public async Task CancelAsync(Guid customerUserId, Guid orderId, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdWithDetailsForCustomerAsync(orderId, customerUserId, ct)
            ?? throw new NotFoundException(MessageKeys.OrderNotFound, "order_not_found");

        if (order.Status is not (OrderStatus.Pending or OrderStatus.Confirmed))
            throw new BadRequestException(MessageKeys.OrderInvalidStatus, "order_invalid_status");

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

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
    }

    private static string? NullIfWhite(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();

    private async Task<Dictionary<Guid, FileAsset>> LoadPrimaryImagesAsync(IEnumerable<Order> orders)
    {
        var ids = orders
            .SelectMany(o => o.Items.Where(i => !i.IsDeleted && i.ProductImageFileId.HasValue).Select(i => i.ProductImageFileId!.Value))
            .Distinct().ToList();
        return await LoadFilesAsync(ids);
    }

    private async Task<Dictionary<Guid, FileAsset>> LoadItemImagesAsync(Order order)
    {
        var ids = order.Items
            .Where(i => !i.IsDeleted && i.ProductImageFileId.HasValue)
            .Select(i => i.ProductImageFileId!.Value)
            .Distinct().ToList();
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
}
