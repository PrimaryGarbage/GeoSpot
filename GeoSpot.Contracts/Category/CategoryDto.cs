namespace GeoSpot.Contracts.Category;

public class CategoryDto
{
    public Guid CategoryId { get; set; }
    
    public required string Name { get; set; }
    
    // raw base64 image data
    public byte[]? IconData { get; set; }
    
    public required string Color { get; set; }
    
    public int SortOrder { get; set; }
}