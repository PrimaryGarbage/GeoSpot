using NetTopologySuite.Geometries;

namespace GeoSpot.Persistence.Entities;

internal enum SpotType { Promo, Event, News, Meetup }

[ExcludeFromCodeCoverage]
internal class SpotEntity : IAuditEntity, IPositionedEntity
{
    public const string TableName = "spots";
    
    public Guid SpotId { get; set; }
    
    public Guid CreatorId { get; set; }
    
    public Guid? BusinessProfileId { get; set; }
    
    public required string Title { get; set; }
    
    public string? Description { get; set; }
    
    public SpotType SpotType { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
    
    public Point Position { get; set; } = Point.Empty;
    
    public int Radius { get; set; }
    
    public string? Address { get; set; }
    
    public DateTime StartsAt { get; set; }
    
    public DateTime EndsAt { get; set; }
    
    public int ViewsCount { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public UserEntity? Creator { get; set; }
    public BusinessProfileEntity? BusinessProfile { get; set; }
    public IEnumerable<CategoryEntity>? Categories { get; set; }
    public IEnumerable<SpotCommentEntity>? Comments { get; set; }
    public IEnumerable<SpotReactionEntity>? Reactions { get; set; }
}