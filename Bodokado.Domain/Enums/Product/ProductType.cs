// Domain/Enums/Product/ProductType.cs
namespace Bodokado.Domain.Enums;

/// <summary>نوع محصول</summary>
public enum ProductType : short
{
    /// <summary>محصول ساده</summary>
    Simple = 1,

    /// <summary>محصول متغیر (با واریانت)</summary>
    Variable = 2
}