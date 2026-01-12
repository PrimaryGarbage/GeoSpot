using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common.Exceptions;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Auth;

public record LogoutUserRequest() : IRequest<Empty>;

public class LogoutUserHandler : IRequestHandler<LogoutUserRequest, Empty>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _userClaimsAccessor;

    public LogoutUserHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor userClaimsAccessor)
    {
        _dbContext = dbContext;
        _userClaimsAccessor = userClaimsAccessor;
    }
    
    public async Task<Empty> Handle(LogoutUserRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _userClaimsAccessor.GetCurrentUserClaims();
        UserEntity _ = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userClaims.UserId, ct)
            ?? throw new NotFoundException($"Failed to find user with the given Id. UserId: {userClaims.UserId}");

        await _dbContext.RefreshTokens.Where(x => x.UserId == userClaims.UserId)
            .ExecuteDeleteAsync(ct);
        
        return Empty.Value;
    }
}