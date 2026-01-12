using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Auth;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Application.Dispatcher.Handlers.Auth;

public record RefreshAccessTokenRequest(string RefreshToken) : IRequest<AccessTokenDto>;

public class RefreshAccessTokenHandler : IRequestHandler<RefreshAccessTokenRequest, AccessTokenDto>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshAccessTokenHandler(GeoSpotDbContext dbContext, IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AccessTokenDto> Handle(RefreshAccessTokenRequest request, CancellationToken ct = default)
    {
        string tokenHash = _jwtTokenService.HashToken(request.RefreshToken);
        
        RefreshTokenEntity? existingRefreshToken = await _dbContext.RefreshTokens.AsNoTracking()
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
        
        if (existingRefreshToken is null) throw new NotFoundException("Failed to find provided refresh token");
        if (existingRefreshToken.Revoked) throw new BadRequestException("Refresh token has already been revoked.");
        if (existingRefreshToken.ExpiresAt < DateTime.UtcNow) throw new BadRequestException("Refresh token has already expired.");
        
        UserEntity existingUser = existingRefreshToken.User!;
        
        string accessToken = _jwtTokenService.GenerateAccessToken(existingUser);
        string refreshToken = _jwtTokenService.GenerateRefreshToken();
        
        await _dbContext.RefreshTokens.Where(x => x.UserId == existingUser.UserId)
            .ExecuteDeleteAsync(ct);
        
        _dbContext.RefreshTokens.Add(new RefreshTokenEntity
        {
            UserId = existingUser.UserId,
            TokenHash = _jwtTokenService.HashToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtTokenService.RefreshTokenLifespanMinutes)
        });
        
        await _dbContext.SaveChangesAsync(ct);
        
        return new AccessTokenDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresInMinutes = _jwtTokenService.AccessTokenLifespanMinutes,
            RefreshToken = refreshToken,
            RefreshTokenExpiresInMinutes = _jwtTokenService.RefreshTokenLifespanMinutes
        };
    }
}