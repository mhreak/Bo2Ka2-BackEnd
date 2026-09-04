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
    }

    public static class Shop
    {
        public const string Auth = $"{ShopBase}/auth";
        public const string Registration = $"{ShopBase}/registration";
        public const string Products = $"{ShopBase}/products";
        public const string Orders = $"{ShopBase}/orders";
        public const string Files = $"{ShopBase}/files";
    }

    public static class Customer
    {
        public const string Auth = $"{CustomerBase}/auth";
        public const string Files = $"{CustomerBase}/files";
        public const string Locations = $"{CustomerBase}/locations";
        public const string Users = $"{CustomerBase}/users";
        public const string Orders = $"{CustomerBase}/orders";
    }

    public static class Corporate
    {
        public const string Auth = $"{CorporateBase}/auth";
        public const string Files = $"{CorporateBase}/files";
        // سفارشات سازمانی و کاتالوگ هدیه بعداً اضافه می‌شود
    }
}