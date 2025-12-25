namespace GeoSpot.Persistence.Entities;

internal class SpotCategoryEntity
{
    public const string TableName = "spot_categories";
    
    public Guid SpotId { get; set; }
    
    public Guid CategoryId { get; set; }
    
    public SpotEntity? Spot { get; set; }
    public CategoryEntity? Category { get; set; }
}