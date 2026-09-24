using Bodokado.Domain.Entities;
using Bodokado.Domain.Entities.Delivery;
using Bodokado.Domain.Entities.Discounts;
using Bodokado.Domain.Entities.Locations;
using Bodokado.Domain.Entities.Order;
using Bodokado.Domain.Entities.Orders;
using Bodokado.Domain.Entities.Organizations;
using Bodokado.Domain.Entities.Products;
using Bodokado.Domain.Entities.Ratings;
using Bodokado.Domain.Entities.Shops;
using Bodokado.Domain.Entities.Stories;
using Bodokado.Domain.Entities.Users;
using Bodokado.Domain.Entities.Wallet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bodokado.Persistence.Context;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<City> Cities => Set<City>();

    public DbSet<FileAsset> Files => Set<FileAsset>();
    public DbSet<Shop> Shops => Set<Shop>();
    public DbSet<ShopCategory> ShopCategories => Set<ShopCategory>();
    public DbSet<ShopWorkingHour> ShopWorkingHours => Set<ShopWorkingHour>();
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
    public DbSet<ProductAttributeValue> ProductAttributeValues => Set<ProductAttributeValue>();
    public DbSet<ProductProductAttribute> Product_ProductAttributes => Set<ProductProductAttribute>();
    public DbSet<OrderCustomizationType> OrderCustomizationTypes => Set<OrderCustomizationType>();
    public DbSet<OrderCustomizationTypeOption> OrderCustomizationTypeOptions => Set<OrderCustomizationTypeOption>();
    public DbSet<ShopOrderCustomizationType> ShopOrderCustomizationTypes => Set<ShopOrderCustomizationType>();
    public DbSet<ShopOrderCustomizationTypeOption> ShopOrderCustomizationTypeOptions => Set<ShopOrderCustomizationTypeOption>();

    public DbSet<EntityRating> EntityRatings => Set<EntityRating>();

    public DbSet<ShopProductVariation> ShopProductVariations => Set<ShopProductVariation>();

    public DbSet<ShopProductVariationProductAttributeValue> ShopProductVariationProductAttributeValues
    => Set<ShopProductVariationProductAttributeValue>();

    public DbSet<DeliveryServiceProvider> DeliveryServiceProviders => Set<DeliveryServiceProvider>();

    public DbSet<ShopOrderDeliveryRule> ShopOrderDeliveryRules => Set<ShopOrderDeliveryRule>();

    public DbSet<WalletTransactionLog> WalletTransactionLogs => Set<WalletTransactionLog>();

    public DbSet<Organization> Organizations => Set<Organization>();

    public DbSet<UserOrganization> UserOrganizations => Set<UserOrganization>();

    public DbSet<OrganizationalGiftCampaign> OrganizationalGiftCampaigns
    => Set<OrganizationalGiftCampaign>();

    public DbSet<Story> Stories => Set<Story>();

    public DbSet<DiscountCode> DiscountCodes => Set<DiscountCode>();


    public DbSet<OrganizationPersonnelCategory> OrganizationPersonnelCategories
    => Set<OrganizationPersonnelCategory>();

    public DbSet<UserOrganizationalGiftCampaign> UserOrganizationalGiftCampaigns
    => Set<UserOrganizationalGiftCampaign>();

    public DbSet<OrganizationalGiftCampaignConstraint> OrganizationalGiftCampaignConstraints
    => Set<OrganizationalGiftCampaignConstraint>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}