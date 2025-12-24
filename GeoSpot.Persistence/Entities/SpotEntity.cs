namespace GeoSpot.Persistence.Entities;

public enum SpotType { Promo, Event, News, Meetup }

public class SpotEntity : BaseAuditExtendedEntity
{
    public Guid Id { get; set; }
    
    public Guid CreatorId { get; set; }
    
    public Guid BusinessProfileId { get; set; }
    
    public required string Title { get; set; }
    
    public string? Description { get; set; }
    
    public SpotType SpotType { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
    
    public int Radius { get; set; }
    
    public string? Address { get; set; }
    
    public DateTime StartsAt { get; set; }
    
    public DateTime EndsAt { get; set; }
    
    public int ViewsCount { get; set; }
    
    public UserEntity? Creator { get; set; }
    public BusinessProfileEntity? BusinessProfile { get; set; }
    public ICollection<CategoryEntity>? Categories { get; set; }
    public ICollection<SpotCommentEntity>? Comments { get; set; }
    public ICollection<SpotReactionEntity>? Reactions { get; set; }
}