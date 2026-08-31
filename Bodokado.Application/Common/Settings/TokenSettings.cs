namespace Bodokado.Application.Common.Settings;

public class TokenSettings
{
    public const string SectionName = "Tokens";
    public int AccessTokenExpirationMinutes { get; set; } = 15;
    public int RefreshTokenExpirationDays { get; set; } = 30;
    public int RefreshTokenAbsoluteExpirationDays { get; set; } = 90;
}
