namespace Bodokado.Application.App.ShopModule.Registration.DTOs;

public class ShopWorkingHourDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public bool IsClosed { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
}