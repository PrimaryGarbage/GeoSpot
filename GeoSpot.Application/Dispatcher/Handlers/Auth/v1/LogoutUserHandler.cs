using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common.Exceptions;
using GeoSpot.Persistence.Repositories.Interfaces;
using GeoSpot.Persistence.Repositories.Models.User;

namespace GeoSpot.Application.Dispatcher.Handlers.Auth.v1;

public record LogoutUserRequest() : IRequest<Empty>;

public class LogoutUserHandler : IRequestHandler<LogoutUserRequest, Empty>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserClaimsAccessor _userClaimsAccessor;
    private readonly IUserRepository _userRepository;

    public LogoutUserHandler(IRefreshTokenRepository refreshTokenRepository, IUserClaimsAccessor userClaimsAccessor, IUserRepository userRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userClaimsAccessor = userClaimsAccessor;
        _userRepository = userRepository;
    }
    
    public async Task<Empty> Handle(LogoutUserRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _userClaimsAccessor.GetCurrentUserClaims();
        UserModel existingUser = await _userRepository.GetUserAsync(userClaims.UserId, ct)
            ?? throw new NotFoundException("Failed to find user with the given ID");

        await _refreshTokenRepository.DeleteAllUserRefreshTokensAsync(existingUser.UserId, ct);
        
        return Empty.Value;
    }
}