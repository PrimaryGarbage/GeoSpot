using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.User;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;

namespace GeoSpot.Application.Dispatcher.Handlers.User;

public record UpdateCurrentUserLocationRequest(UpdateCurrentUserLocationRequestDto Dto) : IRequest<Empty>;

public class UpdateCurrentUserLocationHandler : IRequestHandler<UpdateCurrentUserLocationRequest, Empty>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public UpdateCurrentUserLocationHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<Empty> Handle(UpdateCurrentUserLocationRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _claimsAccessor.GetCurrentUserClaims();
        UserEntity userEntity = await _dbContext.Users.FindAsync([userClaims.UserId], ct)
            ?? throw new NotFoundException($"Failed to find user with the given Id. UserId: {userClaims.UserId}");
        
        userEntity.LastLatitude = request.Dto.Latitude;
        userEntity.LastLongitude = request.Dto.Longitude;
        userEntity.LocationUpdatedAt = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync(ct);
        
        return Empty.Value;
    }
}