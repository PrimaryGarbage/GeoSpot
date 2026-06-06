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
        
        builder.HasData(
            new ReactionTypeEntity
            {
                ReactionTypeId = Guid.Parse("6b9bf263-75f1-439e-8051-d1420e6062a5"),
                Name = "Like",
                Emoji = "❤️",
            },
            new ReactionTypeEntity
            {
                ReactionTypeId = Guid.Parse("8f14bd13-2dda-47db-8df4-0ed445ccb222"),
                Name = "Fire",
                Emoji = "🔥",
            },
            new ReactionTypeEntity
            {
                ReactionTypeId = Guid.Parse("e08c874e-8ae4-4c59-99c8-344c1dd04b18"),
                Name = "Lol",
                Emoji = "😂",
            }
        );
    }
}