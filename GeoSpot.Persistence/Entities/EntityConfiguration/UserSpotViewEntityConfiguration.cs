using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoSpot.Persistence.Entities.EntityConfiguration;

internal class UserSpotViewEntityConfiguration : IEntityTypeConfiguration<UserSpotViewEntity>
{
    public void Configure(EntityTypeBuilder<UserSpotViewEntity> builder)
    {
        builder.ToTable(UserSpotViewEntity.TableName);
        builder.HasKey(x => new { x.UserId, x.SpotId });
        
        builder.HasOne(x => x.Spot)
            .WithMany(x => x.UserViews)
            .HasForeignKey(x => x.SpotId);
        builder.HasOne(x => x.User)
            .WithMany(x => x.SpotViews)
            .HasForeignKey(x => x.UserId);
    }
}