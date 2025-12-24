namespace GeoSpot.Persistence.Entities;

// TODO: Seed DB with starting categories on initial migration
public class CategoryEntity
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    
    // raw base64 image data
    public byte[]? IconData { get; set; }
    
    public required string Color { get; set; }
    
    public int SortOrder { get; set; }
}