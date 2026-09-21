using Bodokado.Domain.Entities;
using Bodokado.Domain.Entities.Locations;
using Bodokado.Domain.Entities.Order;
using Bodokado.Domain.Entities.Orders;
using Bodokado.Domain.Entities.Products;
using Bodokado.Domain.Entities.Shops;
using Bodokado.Domain.Entities.Users;
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
    public DbSet<ProductAttribute> ProductProperties => Set<ProductAttribute>();
    public DbSet<ProductAttributeValue> ProductPropertyValues => Set<ProductAttributeValue>();
    public DbSet<ProductAttributeValue> Product_ProductProperties => Set<ProductAttributeValue>();
    public DbSet<OrderCustomizationType> OrderCustomizationTypes => Set<OrderCustomizationType>();
    public DbSet<OrderCustomizationTypeOption> OrderCustomizationTypeOptions => Set<OrderCustomizationTypeOption>();
    public DbSet<ShopOrderCustomizationType> ShopOrderCustomizationTypes => Set<ShopOrderCustomizationType>();
    public DbSet<ShopOrderCustomizationTypeOption> ShopOrderCustomizationTypeOptions => Set<ShopOrderCustomizationTypeOption>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}