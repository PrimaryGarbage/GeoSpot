namespace GeoSpot.Persistence.Repositories.Models.BusinessProfile;

[ExcludeFromCodeCoverage]
public class CreateBusinessProfileModel
{
    public Guid UserId { get; set; }
    
    public required string Name { get; set; }
    
    public string? Description { get; set; }
    
    public Guid CategoryId { get; set; }
    
    public string? LogoUrl { get; set; }
    
    public string? Address { get; set; }
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
    
    public int Radius { get; set; }
    
    public string? PhoneNumber  { get; set; }
    
    public string? Email { get; set; }
    
    public string? WebsiteUrl { get; set; }
}