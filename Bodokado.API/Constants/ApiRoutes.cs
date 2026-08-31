namespace Bodokado.API.Constants;

public static class ApiRoutes
{
    private const string Api = "api";
    private const string Version = "v1";
    private const string Base = $"{Api}/{Version}";
    private const string AdminBase = $"{Base}/admin";

    public static class Admin
    {
        public const string Auth = $"{AdminBase}/auth";
        public const string Files = $"{AdminBase}/files";
    }

    public static class Generic
    {
        public const string Auth = $"{Base}/auth";
        public const string Files = $"{Base}/files";
        public const string Locations = $"{Base}/locations";
        public const string Users = $"{Base}/users";
    }
}
