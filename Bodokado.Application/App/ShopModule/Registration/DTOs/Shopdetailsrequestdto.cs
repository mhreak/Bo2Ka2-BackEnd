namespace Bodokado.Application.App.ShopModule.Registration.DTOs;

public class ShopDetailsRequestDto
{
    public Guid? AvatarFileId { get; set; }
    public Guid? CoverFileId { get; set; }
    public string TextAddress { get; set; } = default!;
    public Guid? ProvinceId { get; set; }
    public Guid? CityId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}