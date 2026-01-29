using GeoSpot.Application.Mappers;
using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Spot;

public record UpdateSpotRequest(Guid SpotId, UpdateSpotRequestDto Dto) : IRequest<Empty>;

internal class UpdateSpotHandler : IRequestHandler<UpdateSpotRequest, Empty>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public UpdateSpotHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<Empty> Handle(UpdateSpotRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _claimsAccessor.GetCurrentUserClaims();
        UserEntity user = await _dbContext.Users
                              .AsNoTracking()
                              .Include(x => x.BusinessProfiles)
                              .Include(x => x.CreatedSpots)
                              .FirstOrDefaultAsync(x => x.UserId == userClaims.UserId, ct)
            ?? throw new NotFoundException($"Failed to find user with the given ID. UserId: {userClaims.UserId}");
            
        SpotEntity spot = user.CreatedSpots?.FirstOrDefault(x => x.SpotId == request.SpotId)
            ?? throw new NotFoundException($"Failed to find spot with the given ID. UserId: {request.SpotId}");
        
        request.Dto.MapOntoEntity(spot);
        
        await _dbContext.SaveChangesAsync(ct);
        
        return Empty.Value;
    }
}