using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common;
using GeoSpot.Common.Exceptions;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Spot;

public record DeleteSpotRequest(Guid SpotId) : IRequest<Empty>;

internal class DeleteSpotHandler : IRequestHandler<DeleteSpotRequest, Empty>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public DeleteSpotHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<Empty> Handle(DeleteSpotRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _claimsAccessor.GetCurrentUserClaims();
        UserEntity user = await _dbContext.Users
                              .Include(x => x.CreatedSpots)
                              .FirstOrDefaultAsync(x => x.UserId == userClaims.UserId, ct)
            ?? throw new NotFoundException(ErrorMessages.FailedToFindById<UserEntity>(userClaims.UserId));
            
        SpotEntity spot = user.CreatedSpots?.FirstOrDefault(x => x.SpotId == request.SpotId)
            ?? throw new NotFoundException(ErrorMessages.FailedToFindById<SpotEntity>(request.SpotId));
        
        _dbContext.Entry(spot).State = EntityState.Deleted;
        
        await _dbContext.SaveChangesAsync(ct);
        
        return Empty.Value;
    }
}