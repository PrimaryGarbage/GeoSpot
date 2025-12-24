namespace GeoSpot.Persistence.Entities;

public class SpotCategoryEntity
{
    public Guid SpotId { get; set; }
    
    public Guid CategoryId { get; set; }
    
    public SpotEntity? Spot { get; set; }
    public CategoryEntity? Category { get; set; }
}