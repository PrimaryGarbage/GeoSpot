using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoSpot.Persistence.Entities.EntityConfiguration;

internal class SpotCommentEntityConfiguration : IEntityTypeConfiguration<SpotCommentEntity>
{
    public void Configure(EntityTypeBuilder<SpotCommentEntity> builder)
    {
        builder.ToTable(SpotCommentEntity.TableName);
        builder.HasKey(x => x.SpotCommentId);
        builder.Property(x => x.Text).HasMaxLength(-1);
        
        builder.HasOne(x => x.Creator)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.CreatorId);
        builder.HasOne(x => x.Spot)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.SpotId);
    }
}