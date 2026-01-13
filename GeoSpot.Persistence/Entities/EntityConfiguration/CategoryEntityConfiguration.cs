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
        
        builder.HasData(
        [
            new CategoryEntity { CategoryId = new Guid("2a58b9ee-7f22-4df8-b5de-e38f3c3b5a09"), Name = "Еда и рестораны", Color = "#FFFFFF", IconData = [0, 1, 2, 4, 5], SortOrder = 0 },
            new CategoryEntity { CategoryId = new Guid("cad72e62-7293-4bf9-8a2d-178c020031e5"), Name = "Спорт", Color = "#FFFFFF", IconData = [0, 1, 2, 4, 5], SortOrder = 1 },
            new CategoryEntity { CategoryId = new Guid("da1cf683-2dd8-4d88-bc9c-1f0a3cff6920"), Name = "Музыка и концерты", Color = "#FFFFFF", IconData = [0, 1, 2, 4, 5], SortOrder = 2 },
            new CategoryEntity { CategoryId = new Guid("1ac211dc-97e0-409d-955a-6bb33c09b60f"), Name = "Кино", Color = "#FFFFFF", IconData = [0, 1, 2, 4, 5], SortOrder = 3 },
            new CategoryEntity { CategoryId = new Guid("0ac876bb-7930-4024-8810-a5b6f5941f94"), Name = "Шоппинг и скидки", Color = "#FFFFFF", IconData = [0, 1, 2, 4, 5], SortOrder = 4 },
            new CategoryEntity { CategoryId = new Guid("14dc1c16-9cb8-46fb-bd37-03ef4dfa57a6"), Name = "Вечеринки", Color = "#FFFFFF", IconData = [0, 1, 2, 4, 5], SortOrder = 5 },
            new CategoryEntity { CategoryId = new Guid("96eb53bd-bbe3-4b4f-a05a-8c26e29f0dfd"), Name = "Игры", Color = "#FFFFFF", IconData = [0, 1, 2, 4, 5], SortOrder = 6 },
            new CategoryEntity { CategoryId = new Guid("e74b5890-12db-4d0b-8db2-dbeeb8dd7f6b"), Name = "Искусство", Color = "#FFFFFF", IconData = [0, 1, 2, 4, 5], SortOrder = 7 },
            new CategoryEntity { CategoryId = new Guid("d7005c8c-19c8-4ec6-b9d8-1e7e31fb869c"), Name = "Образование", Color = "#FFFFFF", IconData = [0, 1, 2, 4, 5], SortOrder = 8 },
            new CategoryEntity { CategoryId = new Guid("6a5346dc-4606-4142-a217-d603dc687305"), Name = "Другое", Color = "#FFFFFF", IconData = [0, 1, 2, 4, 5], SortOrder = 9 },
        ]);
    }
}