using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common;
using GeoSpot.Common.Exceptions;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Auth;

public record LogoutUserRequest() : IRequest<Empty>;

internal class LogoutUserHandler : IRequestHandler<LogoutUserRequest, Empty>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public LogoutUserHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }
    
    public async Task<Empty> Handle(LogoutUserRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _claimsAccessor.GetCurrentUserClaims();
        UserEntity _ = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userClaims.UserId, ct)
            ?? throw new NotFoundException(ErrorMessages.FailedToFindById<UserEntity>(userClaims.UserId));

        await _dbContext.RefreshTokens.Where(x => x.UserId == userClaims.UserId)
            .ExecuteDeleteAsync(ct);
        
        return Empty.Value;
    }
}