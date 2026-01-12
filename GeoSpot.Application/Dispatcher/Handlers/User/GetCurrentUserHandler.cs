using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Mappers.User;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.User;
using GeoSpot.Persistence.Repositories.Interfaces;
using GeoSpot.Persistence.Repositories.Models.User;

namespace GeoSpot.Application.Dispatcher.Handlers.User;

[ExcludeFromCodeCoverage]
public class GetCurrentUserRequest : IRequest<UserDto>;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserRequest, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserClaimsAccessor _claimsAccessor;

    public GetCurrentUserHandler(IUserRepository userRepository, IUserClaimsAccessor claimsAccessor)
    {
        _userRepository = userRepository;
        _claimsAccessor = claimsAccessor;
    }

    public async Task<UserDto> Handle(GetCurrentUserRequest request, CancellationToken ct = default)
    {
        Guid userId = _claimsAccessor.GetCurrentUserClaims().UserId;
        UserModel user = await _userRepository.GetUserAsync(userId, ct)
            ?? throw new NotFoundException($"Failed to find user with the given id. UserId: {userId}");
        
        return user.MapToDto();
    }
}