using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Bodokado.Domain.Entities;
using Bodokado.Domain.Entities.Locations;
using Bodokado.Domain.Entities.Orders;
using Bodokado.Domain.Entities.Products;
using Bodokado.Domain.Entities.Shops;
using Bodokado.Domain.Entities.Users;

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
    public DbSet<ProductColor> ProductColors => Set<ProductColor>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}