using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoSpot.Persistence.Entities.EntityConfiguration;

internal class SpotReactionEntityConfiguration : IEntityTypeConfiguration<SpotReactionEntity>
{
    public void Configure(EntityTypeBuilder<SpotReactionEntity> builder)
    {
        builder.ToTable(SpotReactionEntity.TableName);
        builder.HasKey(x => new { x.SpotId, UserId = x.CreatorId });
        
        builder.HasOne(x => x.Spot)
            .WithMany(x => x.Reactions)
            .HasForeignKey(x => x.SpotId);
        builder.HasOne(x => x.Creator)
            .WithMany(x => x.Reactions)
            .HasForeignKey(x => x.CreatorId);
        builder.HasOne(x => x.ReactionType)
            .WithMany()
            .HasForeignKey(x => x.ReactionTypeId);
    }
}