namespace Bodokado.Application.App.ShopModule.Registration.DTOs;

public class ShopFinalConfirmationRequestDto
{
    public string ShabaNumber { get; set; } = default!;
    public string ReturnPolicy { get; set; } = default!;
    public List<ShopWorkingHourDto> WorkingHours { get; set; } = new();
}