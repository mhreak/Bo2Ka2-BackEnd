using Bodokado.Application.Administrator.Auth.Interfaces;
using Bodokado.Application.Administrator.Auth.Services;
using Bodokado.Application.App.ShopModule.Auth.Interfaces;
using Bodokado.Application.App.ShopModule.Auth.Services;
using Bodokado.Application.App.ShopModule.Products.Interfaces;
using Bodokado.Application.App.ShopModule.Orders.Interfaces;
using Bodokado.Application.App.ShopModule.Orders.Services;
using Bodokado.Application.App.CustomerModule.Orders.Interfaces;
using Bodokado.Application.App.CustomerModule.Orders.Services;
using Bodokado.Application.App.ShopModule.Products.Services;
using Bodokado.Application.App.ShopModule.Registration.Interfaces;
using Bodokado.Application.App.ShopModule.Registration.Services;
using Bodokado.Application.Common.Auth;
using Bodokado.Application.Common.Auth.Interfaces;
using Bodokado.Application.Common.Auth.Services;
using Bodokado.Application.Common.File.Interfaces;
using Bodokado.Application.Common.File.Services;
using Bodokado.Application.Common.Interfaces;
using Bodokado.Infrastructure.Authentication;
using Bodokado.Persistence.Repositories;
using Bodokado.Persistence.Repositories.File;
using Bodokado.Persistence.Repositories.Products;
using Bodokado.Persistence.Repositories.Orders;
using Bodokado.Persistence.Repositories.Shops;
using Bodokado.Persistence.UnitOfWork;
using Bodokado.Application.Common.Location.Interfaces;
using Bodokado.Persistence.Services;
using Bodokado.Application.Common.Profile.Interfaces;
using Bodokado.Application.Common.Profile.Services;

namespace Bodokado.API.DependencyInjection;

public static class CoreRepositoryDependencyInjection
{
    public static IServiceCollection AddCoreRepositoryDependencies(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFileAssetRepository, FileAssetRepository>();
        services.AddScoped<IShopRepository, ShopRepository>();
        services.AddScoped<IShopCategoryRepository, ShopCategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<RoleAuthCore>();
        services.AddScoped<IRegisterSendOtpService, RegisterSendOtpService>();
        services.AddScoped<IRefreshAccessTokenService, RefreshAccessTokenService>();
        services.AddScoped<IAdminRegisterService, AdminRegisterService>();
        services.AddScoped<IAdminLoginService, AdminLoginService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IGoogleTokenValidator, GoogleTokenValidator>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IShopAuthService, ShopAuthService>();
        services.AddScoped<IShopRegistrationService, ShopRegistrationService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IShopOrderService, ShopOrderService>();
        services.AddScoped<ICustomerOrderService, CustomerOrderService>();

        return services;
    }
}
