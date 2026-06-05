using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common;
using GeoSpot.Common.Exceptions;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Spot;

public record AddSpotViewRequest(Guid SpotId) : IRequest<Empty>;

public class AddSpotViewHandler : IRequestHandler<AddSpotViewRequest, Empty>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public AddSpotViewHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<Empty> Handle(AddSpotViewRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _claimsAccessor.GetCurrentUserClaims();
        
        if (await _dbContext.UserSpotViews.AnyAsync(x => x.UserId == userClaims.UserId && x.SpotId == request.SpotId, ct))
            return Empty.Value;
        
        UserEntity user = await _dbContext.Users
                              .AsNoTracking()
                              .FirstOrDefaultAsync(x => x.UserId == userClaims.UserId, ct)
                          ?? throw new NotFoundException(ErrorMessages.FailedToFindById<UserEntity>(userClaims.UserId));
        
        SpotEntity spot = await _dbContext.Spots
                              .AsNoTracking()
                              .FirstOrDefaultAsync(x => x.SpotId == request.SpotId, ct)
                          ?? throw new NotFoundException(ErrorMessages.FailedToFindById<SpotEntity>(request.SpotId));
        
        _dbContext.UserSpotViews.Add(new UserSpotViewEntity
        {
            UserId = user.UserId, 
            SpotId = spot.SpotId
        });
        
        await _dbContext.SaveChangesAsync(ct);
        
        return Empty.Value;
    }
}