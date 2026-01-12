using GeoSpot.Contracts.User;

namespace GeoSpot.Contracts.Auth;

[ExcludeFromCodeCoverage]
public class VerifyVerificationCodeResponseDto
{
    public required AccessTokenDto Tokens { get; set; }
    
    public UserDto? CreatedUser { get; set; }
}