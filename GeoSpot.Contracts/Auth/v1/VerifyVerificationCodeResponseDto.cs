using GeoSpot.Contracts.User.v1;

namespace GeoSpot.Contracts.Auth.v1;

[ExcludeFromCodeCoverage]
public class VerifyVerificationCodeResponseDto
{
    public required AccessTokenDto Tokens { get; set; }
    
    public UserDto? CreatedUser { get; set; }
}