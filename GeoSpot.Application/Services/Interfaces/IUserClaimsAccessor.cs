using System.Security.Claims;
using GeoSpot.Application.Services.Models;

namespace GeoSpot.Application.Services.Interfaces;

public interface IUserClaimsAccessor
{
    void SetUserClaims(ClaimsPrincipal principal);
    
    UserClaims GetCurrentUserClaims();
}