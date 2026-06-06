using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common;
using GeoSpot.Common.Exceptions;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Device;

public record UnregisterDeviceRequest(Guid TokenId) : IRequest<Empty>;

internal class UnregisterDeviceHandler : IRequestHandler<UnregisterDeviceRequest, Empty>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public UnregisterDeviceHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<Empty> Handle(UnregisterDeviceRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _claimsAccessor.GetCurrentUserClaims();
        
        UserEntity user = await _dbContext.Users
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.UserId == userClaims.UserId, ct)
                          ?? throw new NotFoundException(ErrorMessages.FailedToFindById<UserEntity>(userClaims.UserId));
        
        DeviceTokenEntity deviceToken = await _dbContext.DeviceTokens.FirstOrDefaultAsync(x => x.DeviceTokenId == request.TokenId, ct)
                                        ?? throw new NotFoundException(ErrorMessages.FailedToFindById<DeviceTokenEntity>(request.TokenId));
        
        _dbContext.Entry(deviceToken).State = EntityState.Deleted;
        await _dbContext.SaveChangesAsync(ct);
        
        return Empty.Value;
    }
}