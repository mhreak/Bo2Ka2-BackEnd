namespace Bodokado.Application.App.ShopModule.Registration.DTOs;

public class ShopBasicInfoRequestDto
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string NationalCode { get; set; } = default!;
    public DateTime BirthDate { get; set; }
    public string ShopName { get; set; } = default!;
    public Guid ShopCategoryId { get; set; }
}