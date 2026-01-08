using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Application.Services.Models;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Auth;
using GeoSpot.Persistence.Repositories.Interfaces;
using GeoSpot.Persistence.Repositories.Models.RefreshToken;
using GeoSpot.Persistence.Repositories.Models.User;

namespace GeoSpot.Application.Dispatcher.Handlers.Auth;

[ExcludeFromCodeCoverage]
public record RefreshAccessTokenRequest(string RefreshToken) : IRequest<AccessTokenDto>;

public class RefreshAccessTokenHandler : IRequestHandler<RefreshAccessTokenRequest, AccessTokenDto>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUserRepository _userRepository;
    private readonly IUserClaimsAccessor _userClaimsAccessor;

    public RefreshAccessTokenHandler(IRefreshTokenRepository refreshTokenRepository, IJwtTokenService jwtTokenService, 
        IUserRepository userRepository, IUserClaimsAccessor userClaimsAccessor)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
        _userClaimsAccessor = userClaimsAccessor;
    }

    public async Task<AccessTokenDto> Handle(RefreshAccessTokenRequest request, CancellationToken ct = default)
    {
        UserClaims userClaims = _userClaimsAccessor.GetCurrentUserClaims();
        string tokenHash = _jwtTokenService.HashToken(request.RefreshToken);
        RefreshTokenModel existingRefreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(userClaims.UserId, tokenHash, ct)
            ?? throw new BadRequestException("Failed to find refresh token with the provided info.");
        
        if (existingRefreshToken.Revoked) throw new BadRequestException("Refresh token has already been revoked.");
        if (existingRefreshToken.ExpiresAt < DateTime.UtcNow) throw new BadRequestException("Refresh token has already expired.");
        
        UserModel existingUser = await _userRepository.GetUserAsync(userClaims.UserId, ct)
            ?? throw new NotFoundException("Failed to find user with the provided ID");
        
        string accessToken = _jwtTokenService.GenerateAccessToken(existingUser);
        string refreshToken = _jwtTokenService.GenerateRefreshToken();
        
        await _refreshTokenRepository.DeleteAllUserRefreshTokensAsync(existingUser.UserId, ct);
        
        await _refreshTokenRepository.CreateRefreshTokenAsync(new CreateRefreshTokenModel
        {
            UserId = existingUser.UserId,
            TokenHash = _jwtTokenService.HashToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtTokenService.RefreshTokenLifespanMinutes)
        }, ct);
        
        return new AccessTokenDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresInMinutes = _jwtTokenService.AccessTokenLifespanMinutes,
            RefreshToken = refreshToken,
            RefreshTokenExpiresInMinutes = _jwtTokenService.RefreshTokenLifespanMinutes
        };
    }
}