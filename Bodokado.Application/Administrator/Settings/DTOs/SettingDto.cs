namespace Bodokado.Application.App.AdminModule.Settings.DTOs;

public class SettingDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;

    /// <summary>JSON parse‌شده — نه string خام</summary>
    public object? Value { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class UpsertSettingRequestDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty; // JSON خام
}