namespace Bodokado.API.Constants;

public static class ApiRoutes
{
    private const string Api = "api";
    private const string Version = "v1";
    private const string Base = $"{Api}/{Version}";

    private const string AdminBase = $"{Base}/admin";
    private const string ShopBase = $"{Base}/shop";
    private const string CustomerBase = $"{Base}/customer";
    private const string UserOrganizationBase = $"{Base}/user-organization";

    public static class Admin
    {
        public const string Auth = $"{AdminBase}/auth";
        public const string Files = $"{AdminBase}/files";
        public const string Locations = $"{AdminBase}/Locations";
        public const string ProductCategories = $"{AdminBase}/ProductCategories";
        public const string ProductProperties = $"{AdminBase}/ProductProperties";
        public const string ProductProductProperties = $"{AdminBase}/ProductProductProperties";
        public const string ProductPropertyValues = $"{AdminBase}/ProductPropertyValues";
        public const string Settings = $"{AdminBase}/settings";
        public const string Stories = $"{AdminBase}/stories";
        public const string Banners = $"{AdminBase}/Banners";
    }

    public static class Shop
    {
        public const string Auth = $"{ShopBase}/auth";
        public const string PluginProducts = $"{ShopBase}/PluginProducts";
        public const string Registration = $"{ShopBase}/registration";
        public const string Products = $"{ShopBase}/products";
        public const string Orders = $"{ShopBase}/orders";
        public const string PluginAuth = $"{ShopBase}/PluginAuth";
        public const string Files = $"{ShopBase}/files";
        public const string Locations = $"{ShopBase}/locations";
        public const string Settings = $"{ShopBase}/settings";
        public const string ProductCategories = $"{ShopBase}/ProductCategories";
        public const string ProductProperties = $"{ShopBase}/ProductProperties";
        public const string ProductPropertyValues = $"{ShopBase}/ProductPropertyValues";
        public const string ProductProductProperties = $"{ShopBase}/ProductProductProperties";
        public const string Stories = $"{ShopBase}/stories";
    }

    public static class Customer
    {
        public const string Auth = $"{CustomerBase}/auth";
        public const string Files = $"{CustomerBase}/files";
        public const string Locations = $"{CustomerBase}/locations";
        public const string Users = $"{CustomerBase}/users";
        public const string Orders = $"{CustomerBase}/orders";
        public const string Shops = $"{CustomerBase}/shops";
        public const string Banners = $"{CustomerBase}/Banners";
        public const string Products = $"{CustomerBase}/products";
        public const string ProductCategories = $"{CustomerBase}/ProductCategories";
        public const string ProductProperties = $"{CustomerBase}/ProductProperties";
        public const string ProductPropertyValues = $"{CustomerBase}/ProductPropertyValues";
        public const string ProductProductProperties = $"{CustomerBase}/ProductProductProperties";
        public const string Stories = $"{CustomerBase}/stories";

        public const string Settings = $"{CustomerBase}/settings";
    }

    public static class UserOrganization
    {
        public const string Auth = $"{UserOrganizationBase}/auth";
        public const string ProductPropertyValues = $"{UserOrganizationBase}/ProductPropertyValues";
        public const string Files = $"{UserOrganizationBase}/files";
        public const string ProductProductProperties = $"{UserOrganizationBase}/ProductProductProperties";
        public const string Locations = $"{UserOrganizationBase}/Locations";
        public const string ProductProperties = $"{UserOrganizationBase}/ProductProperties";
        public const string ProductCategories = $"{UserOrganizationBase}/ProductCategories";
        public const string Settings = $"{UserOrganizationBase}/settings";
        // سفارشات سازمانی و کاتالوگ هدیه بعداً اضافه می‌شود
    }
}