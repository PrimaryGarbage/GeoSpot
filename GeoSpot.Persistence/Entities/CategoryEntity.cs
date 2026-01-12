namespace GeoSpot.Persistence.Entities;

// TODO: Add additional table with category name translations for i18n
public class CategoryEntity
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