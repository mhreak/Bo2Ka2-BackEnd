namespace Bodokado.Application.Common.Otp;

public enum OtpGenerationStatus { Sent, CooldownActive }

public class OtpGenerationResult
{
    public OtpGenerationStatus Status { get; init; }
    public int? RetryAfterSeconds { get; init; }
    public string? Code { get; init; }
    public static OtpGenerationResult Sent(string? code = null) => new() { Status = OtpGenerationStatus.Sent, Code = code };
    public static OtpGenerationResult Cooldown(int retryAfterSeconds) => new() { Status = OtpGenerationStatus.CooldownActive, RetryAfterSeconds = retryAfterSeconds };
}

public enum OtpVerificationStatus { Success, NotFoundOrExpired, InvalidCode, MaxAttemptsExceeded }

public class OtpVerificationResult
{
    public OtpVerificationStatus Status { get; init; }
    public int? RemainingAttempts { get; init; }
    public static OtpVerificationResult Success() => new() { Status = OtpVerificationStatus.Success };
    public static OtpVerificationResult NotFoundOrExpired() => new() { Status = OtpVerificationStatus.NotFoundOrExpired };
    public static OtpVerificationResult MaxAttemptsExceeded() => new() { Status = OtpVerificationStatus.MaxAttemptsExceeded };
    public static OtpVerificationResult InvalidCode(int remainingAttempts) => new() { Status = OtpVerificationStatus.InvalidCode, RemainingAttempts = remainingAttempts };
}
