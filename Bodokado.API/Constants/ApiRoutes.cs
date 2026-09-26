namespace Bodokado.API.Constants;

public static class ApiRoutes
{
    private const string Api = "api";
    private const string Version = "v1";
    private const string Base = $"{Api}/{Version}";

    private const string AdminBase = $"{Base}/admin";
    private const string ShopBase = $"{Base}/shop";
    private const string CustomerBase = $"{Base}/customer";
    private const string CorporateBase = $"{Base}/corporate";

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
    }

    public static class Shop
    {
        public const string Auth = $"{ShopBase}/auth";
        public const string Registration = $"{ShopBase}/registration";
        public const string Products = $"{ShopBase}/products";
        public const string Orders = $"{ShopBase}/orders";
        public const string Files = $"{ShopBase}/files";
        public const string Locations = $"{ShopBase}/locations";
        public const string Settings = $"{ShopBase}/settings";
        public const string ProductCategories = $"{ShopBase}/ProductCategories";
        public const string ProductProperties = $"{ShopBase}/ProductProperties";
        public const string ProductPropertyValues = $"{ShopBase}/ProductPropertyValues";
        public const string ProductProductProperties = $"{ShopBase}/ProductProductProperties";
    }

    public static class Customer
    {
        public const string Auth = $"{CustomerBase}/auth";
        public const string Files = $"{CustomerBase}/files";
        public const string Locations = $"{CustomerBase}/locations";
        public const string Users = $"{CustomerBase}/users";
        public const string Orders = $"{CustomerBase}/orders";
        public const string ProductCategories = $"{CustomerBase}/ProductCategories";
        public const string ProductProperties = $"{CustomerBase}/ProductProperties";
        public const string ProductPropertyValues = $"{CustomerBase}/ProductPropertyValues";
        public const string ProductProductProperties = $"{CustomerBase}/ProductProductProperties";

        public const string Settings = $"{CustomerBase}/settings";
    }

    public static class Corporate
    {
        public const string Auth = $"{CorporateBase}/auth";
        public const string ProductPropertyValues = $"{CorporateBase}/ProductPropertyValues";
        public const string Files = $"{CorporateBase}/files";
        public const string ProductProductProperties = $"{CorporateBase}/ProductProductProperties";
        public const string Locations = $"{CorporateBase}/Locations";
        public const string ProductProperties = $"{CorporateBase}/ProductProperties";
        public const string ProductCategories = $"{CorporateBase}/ProductCategories";
        public const string Settings = $"{CorporateBase}/settings";
        // سفارشات سازمانی و کاتالوگ هدیه بعداً اضافه می‌شود
    }
}