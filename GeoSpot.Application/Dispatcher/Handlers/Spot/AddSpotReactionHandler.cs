using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Spot;

public record AddSpotReactionRequest(Guid SpotId, AddSpotReactionRequestDto Dto) : IRequest<Empty>;

internal class AddSpotReactionHandler : IRequestHandler<AddSpotReactionRequest, Empty>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public AddSpotReactionHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<Empty> Handle(AddSpotReactionRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _claimsAccessor.GetCurrentUserClaims();
        
        UserEntity user = await _dbContext.Users
                              .AsNoTracking()
                              .FirstOrDefaultAsync(x => x.UserId == userClaims.UserId, ct)
                          ?? throw new NotFoundException(ErrorMessages.FailedToFindById<UserEntity>(userClaims.UserId));
        
        ReactionTypeEntity reactionType = await _dbContext.ReactionTypes
                              .AsNoTracking()
                              .FirstOrDefaultAsync(x => x.ReactionTypeId == request.Dto.ReactionTypeId, ct)
                          ?? throw new NotFoundException(ErrorMessages.FailedToFindById<ReactionTypeEntity>(request.Dto.ReactionTypeId));
        
        SpotReactionEntity? reaction = await _dbContext.SpotReactions.FindAsync([request.SpotId, userClaims.UserId], ct);
        
        if (reaction is null)
        {
            SpotEntity spot = await _dbContext.Spots
                                .AsNoTracking()
                                .FirstOrDefaultAsync(x => x.SpotId == request.SpotId, ct) 
                              ?? throw new NotFoundException(ErrorMessages.FailedToFindById<SpotEntity>(request.SpotId));
                
            _dbContext.SpotReactions.Add(new SpotReactionEntity
            {
                CreatorId = userClaims.UserId,
                SpotId = spot.SpotId,
                ReactionTypeId = reactionType.ReactionTypeId
            });
        }
        else
        {
            reaction.ReactionTypeId = reactionType.ReactionTypeId;
        }
        
        await _dbContext.SaveChangesAsync(ct);
        
        return Empty.Value;
    }
}