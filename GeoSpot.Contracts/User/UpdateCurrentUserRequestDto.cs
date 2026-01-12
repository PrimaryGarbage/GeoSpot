using GeoSpot.Common.Enums;

namespace GeoSpot.Contracts.User;


public class UpdateCurrentUserRequestDto
{
    public string? Email { get; set; }

    public int DetectionRadius { get; set; }
    
    public required string DisplayName { get; set; }
    
    public string? AvatarUrl { get; set; }
    
    public int BirthYear { get; set; }

    public Gender Gender { get; set; }
}