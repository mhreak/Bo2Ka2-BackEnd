// Persistence/Configurations/Rating/EntityRatingConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bodokado.Domain.Entities.Ratings;

namespace Bodokado.Persistence.Configurations.Ratings;

public class EntityRatingConfiguration : IEntityTypeConfiguration<EntityRating>
{
    public void Configure(EntityTypeBuilder<EntityRating> builder)
    {
        builder.ToTable("EntityRating");
        builder.HasKey(x => new { x.EntityId, x.RatingOptionId });

        builder.Property(x => x.Score).IsRequired();
        builder.Property(x => x.InsertDateTime).IsRequired();

        builder.HasOne(x => x.RatingOption)
            .WithMany()
            .HasForeignKey(x => x.RatingOptionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.EntityId);
        builder.HasIndex(x => x.InsertDateTime);
    }
}