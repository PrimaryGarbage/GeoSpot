using GeoSpot.Application.Mappers.User;
using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.User;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.User;

public class GetCurrentUserRequest : IRequest<UserDto>;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserRequest, UserDto>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public GetCurrentUserHandler(GeoSpotDbContext dbContext, IUserClaimsAccessor claimsAccessor)
    {
        _dbContext = dbContext;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<UserDto> Handle(GetCurrentUserRequest request, CancellationToken ct = default)
    {
        Guid userId = _claimsAccessor.GetCurrentUserClaims().UserId;
        UserEntity user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId, ct)
            ?? throw new NotFoundException($"Failed to find user with the given id. UserId: {userId}");
        
        return user.MapToDto();
    }
}