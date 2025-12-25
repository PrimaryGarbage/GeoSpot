using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoSpot.Persistence.Entities.EntityConfiguration;

internal class SpotEntityConfiguration : IEntityTypeConfiguration<SpotEntity>
{
    public void Configure(EntityTypeBuilder<SpotEntity> builder)
    {
        builder.ToTable(SpotEntity.TableName);
        builder.HasKey(x => x.SpotId);
        builder.Property(x => x.Title).HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(-1);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.Property(x => x.Latitude).HasPrecision(10, 8);
        builder.Property(x => x.Longitude).HasPrecision(11, 8);
        builder.Property(x => x.Address).HasMaxLength(500);
        
        builder.HasOne(x => x.Creator)
            .WithMany(x => x.CreatedSpots)
            .HasForeignKey(x => x.CreatorId);
        builder.HasOne(x => x.BusinessProfile)
            .WithMany(x => x.Spots)
            .HasForeignKey(x => x.BusinessProfileId)
            .IsRequired(false);
        builder.HasMany(x => x.Categories)
            .WithMany()
            .UsingEntity<SpotCategoryEntity>()
            .ToTable(SpotCategoryEntity.TableName)
            .HasKey(x => new { x.SpotId, x.CategoryId });
        builder.HasMany(x => x.Comments)
            .WithOne(x => x.Spot);
        builder.HasMany(x => x.Reactions)
            .WithOne(x => x.Spot);
    }
}