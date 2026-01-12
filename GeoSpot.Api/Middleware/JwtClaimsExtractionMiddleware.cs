using GeoSpot.Application.Services.Interfaces;

namespace GeoSpot.Api.Middleware;

public class JwtClaimsExtractionMiddleware
{
    private readonly RequestDelegate _next;

    public JwtClaimsExtractionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task Invoke(HttpContext context, IUserClaimsAccessor userClaimsAccessor)
    {
        if(context.User.Identity?.IsAuthenticated == true)
            userClaimsAccessor.SetUserClaims(context.User);
        
        await _next(context);
    }
}