namespace Bodokado.Application.Common.Otp;

public class OtpSettings
{
    public const string SectionName = "Otp";
    public int CodeLength { get; set; } = 5;
    public int ExpirationMinutes { get; set; } = 2;
    public int MaxAttempts { get; set; } = 5;
    public int ResendCooldownSeconds { get; set; } = 60;
    public bool ExposeCodeInResponse { get; set; } = false;
}
