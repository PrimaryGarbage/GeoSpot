using GeoSpot.Common.Enums;

namespace GeoSpot.Application.Services.Models;

[ExcludeFromCodeCoverage]
public class UserClaims
{
    public Guid UserId { get; set; }
    
    public required string PhoneNumber { get; set; }
    
    public string? Email { get; set; }
    
    public UserRole Role { get; set; }
}