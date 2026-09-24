// Domain/Enums/Story/StoryButtonClickActionType.cs
namespace Bodokado.Domain.Enums;

/// <summary>نوع عملیات کلیک دکمه استوری</summary>
public enum StoryButtonClickActionType : short
{
    None = 0,
    OpenProduct = 1,
    OpenShop = 2,
    OpenUrl = 3,
    OpenCategory = 4
}