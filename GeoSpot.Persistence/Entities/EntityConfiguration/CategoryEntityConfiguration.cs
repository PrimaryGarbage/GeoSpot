using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoSpot.Persistence.Entities.EntityConfiguration;

internal class CategoryEntityConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    public void Configure(EntityTypeBuilder<CategoryEntity> builder)
    {
        builder.ToTable(CategoryEntity.TableName);
        builder.HasKey(x => x.CategoryId);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.IconData).HasColumnType("bytea");
        builder.Property(x => x.Color).HasMaxLength(7);
        
        builder.HasMany(x => x.Users)
            .WithMany()
            .UsingEntity<UserCategoryEntity>()
            .ToTable(UserCategoryEntity.TableName)
            .HasKey(x => new { x.UserId, x.CategoryId });
        
        // TODO: Seed default categories
    }
}