using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common;
using GeoSpot.Common.Exceptions;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Spot;

public record RemoveSpotCommentRequest(Guid CommentId) : IRequest<Empty>;

internal class RemoveSpotCommentHandler : IRequestHandler<RemoveSpotCommentRequest, Empty>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public RemoveSpotCommentHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<Empty> Handle(RemoveSpotCommentRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _claimsAccessor.GetCurrentUserClaims();
        
        UserEntity user = await _dbContext.Users
                              .AsNoTracking()
                              .FirstOrDefaultAsync(x => x.UserId == userClaims.UserId, ct)
                          ?? throw new NotFoundException(ErrorMessages.FailedToFindById<UserEntity>(userClaims.UserId));
        
        SpotCommentEntity comment = await _dbContext.SpotComments
                                        .FirstOrDefaultAsync(x => x.SpotCommentId == request.CommentId && x.CreatorId == user.UserId, ct)
                                    ?? throw new NotFoundException(ErrorMessages.FailedToFindById<SpotCommentEntity>(request.CommentId));
        
        _dbContext.Entry(comment).State = EntityState.Deleted;
        await _dbContext.SaveChangesAsync(ct);
        
        return Empty.Value;
    }
}