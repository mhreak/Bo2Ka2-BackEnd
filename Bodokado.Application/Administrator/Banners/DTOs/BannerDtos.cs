// Application/Administrator/Banners/DTOs/BannerDtos.cs
namespace Bodokado.Application.Administrator.Banners.DTOs;

public class BannerDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ImageId { get; set; }
    public string? ImagePath { get; set; }
    public string? Link { get; set; }
    public short ShowOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateBannerRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ImageId { get; set; }
    public string? Link { get; set; }
    public short ShowOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateBannerRequestDto : CreateBannerRequestDto { }