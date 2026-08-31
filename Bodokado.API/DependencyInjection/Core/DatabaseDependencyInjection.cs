using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bodokado.Application.Common.Interfaces;
using Bodokado.Domain.Entities.Users;
using Bodokado.Persistence.Context;
using Bodokado.Persistence.Seeders;
using Bodokado.Persistence.UnitOfWork;

namespace Bodokado.API.DependencyInjection;

public static class DatabaseDependencyInjection
{
    public static IServiceCollection AddDatabaseDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }

    public static async Task MigrateAndSeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        const int retryCount = 10;
        for (var i = 0; i < retryCount; i++)
        {
            try
            {
                await dbContext.Database.MigrateAsync();
                break;
            }
            catch
            {
                if (i == retryCount - 1) throw;
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        await LocationSeeder.SeedAsync(dbContext);
        await AdminSeeder.SeedAsync(dbContext, userManager, roleManager);
        await ShopCategorySeeder.SeedAsync(dbContext);
    }
}