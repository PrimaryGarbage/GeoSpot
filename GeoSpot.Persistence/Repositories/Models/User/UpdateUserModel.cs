namespace GeoSpot.Persistence.Repositories.Models.User;

[ExcludeFromCodeCoverage]
public class UpdateUserModel
{
    public required string PhoneNumber { get; set; }

    public string? Email { get; set; }

    public bool IsVerified { get; set; }
    
    public int DetectionRadius { get; set; }
    
    public bool IsPremium { get; set; }
    
    public required string DisplayName { get; set; }
    
    public string? AvatarUrl { get; set; }
}