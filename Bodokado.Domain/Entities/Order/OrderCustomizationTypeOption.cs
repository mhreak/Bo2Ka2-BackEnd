using Bodokado.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bodokado.Domain.Entities.Order;

public class OrderCustomizationTypeOption : BaseEntity
{
    public string OptionName { get; set; } = string.Empty;

    public Guid? ImageFileId { get; set; }
    public FileAsset? ImageFile { get; set; }

    public Guid? ThumbnailImageFileId { get; set; }
    public FileAsset? ThumbnailImageFile { get; set; }

    public short ShowOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid OrderCustomizationTypeId { get; set; }
    public OrderCustomizationType OrderCustomizationType { get; set; } = null!;

    public List<ShopOrderCustomizationTypeOption> ShopLinks { get; set; } = new();
}
