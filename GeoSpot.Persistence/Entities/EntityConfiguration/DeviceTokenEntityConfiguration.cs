using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoSpot.Persistence.Entities.EntityConfiguration;

internal class DeviceTokenEntityConfiguration : IEntityTypeConfiguration<DeviceTokenEntity>
{
    public void Configure(EntityTypeBuilder<DeviceTokenEntity> builder)
    {
        builder.ToTable(DeviceTokenEntity.TableName);
        builder.HasKey(x => x.DeviceTokenId);
        builder.Property(x => x.Token).HasMaxLength(500);
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.DeviceTokens)
            .HasForeignKey(x => x.UserId);
    }
}