using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common;
using GeoSpot.Common.Exceptions;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Spot;

public record RemoveSpotReactionRequest(Guid SpotId) : IRequest<Empty>;

internal class RemoveSpotReactionHandler : IRequestHandler<RemoveSpotReactionRequest, Empty>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public RemoveSpotReactionHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<Empty> Handle(RemoveSpotReactionRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _claimsAccessor.GetCurrentUserClaims();
        
        SpotReactionEntity spotReaction = await _dbContext.SpotReactions.FirstOrDefaultAsync(x => x.SpotId == request.SpotId && x.CreatorId == userClaims.UserId, ct)
            ?? throw new NotFoundException(ErrorMessages.FailedToFindById<SpotReactionEntity>(request.SpotId));
        
        _dbContext.Entry(spotReaction).State = EntityState.Deleted;
        
        await _dbContext.SaveChangesAsync(ct);
        
        return Empty.Value;
    }
}