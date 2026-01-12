namespace GeoSpot.Persistence.Entities;

public class UserCategoryEntity
{
    public const string TableName = "user_categories";
    
    public Guid UserId { get; set; }
    
    public Guid CategoryId { get; set; }
}