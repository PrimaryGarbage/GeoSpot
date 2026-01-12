using GeoSpot.Application.Mappers.User;
using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.User;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;

namespace GeoSpot.Application.Dispatcher.Handlers.User;


public record UpdateCurrentUserRequest(UpdateCurrentUserRequestDto Dto) : IRequest<UserDto>;

public class UpdateCurrentUserHandler : IRequestHandler<UpdateCurrentUserRequest, UserDto>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public UpdateCurrentUserHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<UserDto> Handle(UpdateCurrentUserRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _claimsAccessor.GetCurrentUserClaims();
        UserEntity userEntity = await _dbContext.Users.FindAsync([userClaims.UserId], ct)
            ?? throw new NotFoundException($"Failed to find user with the given Id: {userClaims.UserId}");
        
        request.Dto.MapOntoEntity(userEntity);
        
        await _dbContext.SaveChangesAsync(ct);
        
        return userEntity.MapToDto();
    }
}