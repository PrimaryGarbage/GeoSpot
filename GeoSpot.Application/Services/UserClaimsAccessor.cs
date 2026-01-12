using System.Security.Claims;
using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common.Constants;
using GeoSpot.Common.Enums;
using GeoSpot.Common.Exceptions;

namespace GeoSpot.Application.Services;

[ExcludeFromCodeCoverage]
internal class UserClaimsAccessor : IUserClaimsAccessor
{
    private UserClaims? _currentUserClaims;

    public void SetUserClaims(ClaimsPrincipal principal)
    {
        string userIdStr = principal.FindFirst(ClaimsConstants.UserId)?.Value
            ?? throw new InternalProblemException("Failed to extract UserId claim from the ClaimsPrincipal");
        
        Guid userId = Guid.Parse(userIdStr);
        if(userId == Guid.Empty) 
            throw new InternalProblemException("Failed to parse UserId from the ClaimsPrincipal into Guid");
        
        string phoneNumber = principal.FindFirst(ClaimsConstants.PhoneNumber)?.Value
            ?? throw new InternalProblemException("Failed to extract PhoneNumber claim from the ClaimsPrincipal");
        
        string? email = principal.FindFirst(ClaimsConstants.PhoneNumber)?.Value;

        string userRoleStr = principal.FindFirst(ClaimsConstants.Role)?.Value 
            ?? throw new InternalProblemException("Failed to extract Role claim from the ClaimsPrincipal");
        if(!Enum.TryParse(userRoleStr, out UserRole userRole))
            throw new InternalProblemException("Failed to parse Role claim from the ClaimsPrincipal into the UserRole enum");
        
        _currentUserClaims = new UserClaims
        {
            UserId = userId,
            PhoneNumber = phoneNumber,
            Email = email,
            Role = userRole
        };
    }

    public UserClaims GetCurrentUserClaims()
    {
        return _currentUserClaims ?? throw new InternalProblemException("Trying to access uninitialized UserClaims instance from the UserClaimsAccessor");
    }
}