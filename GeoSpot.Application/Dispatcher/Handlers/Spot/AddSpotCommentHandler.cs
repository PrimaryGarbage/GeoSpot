using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Spot;

public record AddSpotCommentRequest(Guid SpotId, AddSpotCommentRequestDto Dto) : IRequest<AddSpotCommentResponseDto>;

internal class AddSpotCommentHandler : IRequestHandler<AddSpotCommentRequest, AddSpotCommentResponseDto>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public AddSpotCommentHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<AddSpotCommentResponseDto> Handle(AddSpotCommentRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _claimsAccessor.GetCurrentUserClaims();
        
        UserEntity user = await _dbContext.Users
                              .AsNoTracking()
                              .FirstOrDefaultAsync(x => x.UserId == userClaims.UserId, ct)
                          ?? throw new NotFoundException(ErrorMessages.FailedToFindById<UserEntity>(userClaims.UserId));
        
        SpotEntity spot = await _dbContext.Spots
                              .AsNoTracking()
                              .FirstOrDefaultAsync(x => x.SpotId == request.SpotId, ct)
                          ?? throw new NotFoundException(ErrorMessages.FailedToFindById<SpotEntity>(request.SpotId));
        
        SpotCommentEntity comment = new()
        {
            SpotId = spot.SpotId,
            CreatorId = user.UserId,
            Text = request.Dto.Text,
        };
        
        _dbContext.SpotComments.Add(comment);
        await _dbContext.SaveChangesAsync(ct);
        
        return new AddSpotCommentResponseDto(comment.SpotCommentId);
    }
}