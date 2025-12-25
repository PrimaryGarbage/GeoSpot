namespace GeoSpot.Persistence.Entities;

// TODO: Seed DB with starting categories on initial migration
// TODO: Add additional table with category name translations for i18n
[ExcludeFromCodeCoverage]
internal class CategoryEntity
{
    public const string TableName = "categories";
    
    public Guid CategoryId { get; set; }
    
    public required string Name { get; set; }
    
    // raw base64 image data
    public byte[]? IconData { get; set; }
    
    public required string Color { get; set; }
    
    public int SortOrder { get; set; }
    
    public IEnumerable<UserEntity>? Users { get; set; }
}