namespace GeoSpot.Persistence.Entities;

public class UserCategoryEntity
{
    public Guid UserId { get; set; }
    
    public Guid CategoryId { get; set; }
    
    public UserEntity? User { get; set; }
    public CategoryEntity? Category { get; set; }
}