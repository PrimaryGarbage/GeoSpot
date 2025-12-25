using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoSpot.Persistence.Entities.EntityConfiguration;

internal class BusinessProfileEntityConfiguration : IEntityTypeConfiguration<BusinessProfileEntity>
{
    public void Configure(EntityTypeBuilder<BusinessProfileEntity> builder)
    {
        builder.ToTable(BusinessProfileEntity.TableName);
        builder.HasKey(x => x.BusinessProfileId);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(-1);
        builder.Property(x => x.LogoUrl).HasMaxLength(500);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.Latitude).HasPrecision(10, 8);
        builder.Property(x => x.Longitude).HasPrecision(11, 8);
        builder.Property(x => x.PhoneNumber).HasMaxLength(50);
        builder.Property(x => x.Email).HasMaxLength(256);
        builder.Property(x => x.WebsiteUrl).HasMaxLength(500);
        
        builder.HasOne(x => x.Creator)
            .WithMany(x => x.BusinessProfiles)
            .HasForeignKey(x => x.UserId);
        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey((x => x.CategoryId));
        builder.HasMany(x => x.Spots)
            .WithOne(x => x.BusinessProfile);
    }
}