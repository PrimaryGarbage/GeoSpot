using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoSpot.Persistence.Entities.EntityConfiguration;

internal class ReactionTypeEntityConfiguration : IEntityTypeConfiguration<ReactionTypeEntity>
{
    public void Configure(EntityTypeBuilder<ReactionTypeEntity> builder)
    {
        builder.ToTable(ReactionTypeEntity.TableName);
        builder.HasKey(x => x.ReactionTypeId);
        builder.Property(x => x.Name).HasMaxLength(20);
        builder.Property(x => x.Emoji).HasMaxLength(20);
    }
}