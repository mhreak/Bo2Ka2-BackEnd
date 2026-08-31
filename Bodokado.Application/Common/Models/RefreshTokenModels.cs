namespace Bodokado.Application.Common.Models;

public class RefreshTokenIssueResult
{
    public string RefreshToken { get; init; } = default!;
    public Guid SessionId { get; init; }
    public DateTime ExpiresAtUtc { get; init; }
}

public enum RefreshTokenRotationStatus
{
    Success,
    Invalid
}

public class RefreshTokenRotationResult
{
    public RefreshTokenRotationStatus Status { get; init; }
    public Guid UserId { get; init; }
    public Guid SessionId { get; init; }
    public string? NewRefreshToken { get; init; }
    public string? Role { get; set; }
    public DateTime? NewExpiresAtUtc { get; init; }

    public static RefreshTokenRotationResult Success(Guid userId, Guid sessionId, string newRefreshToken, DateTime newExpiresAtUtc, string role) =>
        new() { Status = RefreshTokenRotationStatus.Success, UserId = userId, SessionId = sessionId, NewRefreshToken = newRefreshToken, NewExpiresAtUtc = newExpiresAtUtc, Role = role };

    public static RefreshTokenRotationResult Invalid() => new() { Status = RefreshTokenRotationStatus.Invalid };
}
