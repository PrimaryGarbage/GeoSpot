using GeoSpot.Application.Services.Interfaces;
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
    private readonly IUnitOfWork _unitOfWork;

    public RefreshAccessTokenHandler(IRefreshTokenRepository refreshTokenRepository, IJwtTokenService jwtTokenService, 
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AccessTokenDto> Handle(RefreshAccessTokenRequest request, CancellationToken ct = default)
    {
        string tokenHash = _jwtTokenService.HashToken(request.RefreshToken);
        (RefreshTokenModel? existingRefreshToken, UserModel? user) = await _refreshTokenRepository.GetRefreshTokenWithUserAsync(tokenHash, ct);
        
        if (existingRefreshToken is null) throw new NotFoundException("Failed to find provided refresh token");
        if (user is null) throw new NotFoundException("Failed to find user attached to the provided refresh token");
        if (existingRefreshToken.Revoked) throw new BadRequestException("Refresh token has already been revoked.");
        if (existingRefreshToken.ExpiresAt < DateTime.UtcNow) throw new BadRequestException("Refresh token has already expired.");
        
        string accessToken = _jwtTokenService.GenerateAccessToken(user);
        string refreshToken = _jwtTokenService.GenerateRefreshToken();
        
        _unitOfWork.Start();
        
        await _refreshTokenRepository.DeleteAllUserRefreshTokensAsync(user.UserId, ct);
        
        await _refreshTokenRepository.CreateRefreshTokenAsync(new CreateRefreshTokenModel
        {
            UserId = user.UserId,
            TokenHash = _jwtTokenService.HashToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtTokenService.RefreshTokenLifespanMinutes)
        }, ct);
        
        await _unitOfWork.CommitAsync(ct);
        
        return new AccessTokenDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresInMinutes = _jwtTokenService.AccessTokenLifespanMinutes,
            RefreshToken = refreshToken,
            RefreshTokenExpiresInMinutes = _jwtTokenService.RefreshTokenLifespanMinutes
        };
    }
}