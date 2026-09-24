using Bodokado.Domain.Entities.Shops;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ShopProductImageConfiguration : IEntityTypeConfiguration<ShopProductImage>
{
    public void Configure(EntityTypeBuilder<ShopProductImage> builder)
    {
        builder.ToTable("ShopProductImage");
        builder.HasKey(x => x.Id);

        // Product → Images
        builder.HasOne(x => x.Product)           // x = ShopProductImage
            .WithMany(p => p.Images)             // p = Product  →  Images نه Product
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // FileAsset
        builder.HasOne(x => x.File)              // x = ShopProductImage
            .WithMany()                          // File روی Product نیست
            .HasForeignKey(x => x.FileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}