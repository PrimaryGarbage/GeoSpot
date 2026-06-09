using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Device;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Device;

public record RegisterDeviceRequest(RegisterDeviceRequestDto Dto) : IRequest<RegisterDeviceResponseDto>;

internal class RegisterDeviceHandler : IRequestHandler<RegisterDeviceRequest, RegisterDeviceResponseDto>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public RegisterDeviceHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<RegisterDeviceResponseDto> Handle(RegisterDeviceRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _claimsAccessor.GetCurrentUserClaims();
        
        UserEntity user = await _dbContext.Users
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.UserId == userClaims.UserId, ct)
                          ?? throw new NotFoundException(ErrorMessages.FailedToFindById<UserEntity>(userClaims.UserId));
        
        if (await _dbContext.DeviceTokens.AnyAsync(x => x.UserId == user.UserId && x.Token == request.Dto.Token, ct))
            throw new BadRequestException("Provided device has already been registered");
        
        DeviceTokenEntity deviceToken = new()
        {
            UserId = user.UserId,
            Token = request.Dto.Token,
            Platform = request.Dto.Platform,
            IsActive = true,
        };
        
        _dbContext.DeviceTokens.Add(deviceToken);
        await _dbContext.SaveChangesAsync(ct);
        
        return new RegisterDeviceResponseDto(deviceToken.DeviceTokenId);
    }
}