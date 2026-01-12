using GeoSpot.Application.Mappers.User;
using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Common;
using GeoSpot.Common.ConfigurationSections;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Auth;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Entities;
using GeoSpot.Persistence.Entities.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GeoSpot.Application.Dispatcher.Handlers.Auth;

public record VerifyVerificationCodeRequest(VerifyVerificationCodeRequestDto Dto) : IRequest<VerifyVerificationCodeResponseDto>;

public class VerifyVerificationCodeHandler : IRequestHandler<VerifyVerificationCodeRequest, VerifyVerificationCodeResponseDto>
{
    private readonly GeoSpotDbContext _dbContext;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly VerificationCodeConfigurationSection _configuration;
    private readonly ICacheService _cacheService;

    public VerifyVerificationCodeHandler(GeoSpotDbContext dbContext, IJwtTokenService jwtTokenService, 
        IOptions<VerificationCodeConfigurationSection> options, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
        _cacheService = cacheService;
        _configuration = options.Value;
    }

    public async Task<VerifyVerificationCodeResponseDto> Handle(VerifyVerificationCodeRequest request, CancellationToken ct)
    {
        string cacheKey = CacheKeys.VerificationCodeEntity(request.Dto.PhoneNumber);
        
        VerificationCodeEntity existingCode = await _cacheService.GetAsync<VerificationCodeEntity>(cacheKey, ct)
            ?? await _dbContext.VerificationCodes.AsNoTracking().FirstOrDefaultAsync(x => 
                x.PhoneNumber == request.Dto.PhoneNumber && x.VerificationCode == request.Dto.VerificationCode, ct)
            ?? throw new NotFoundException("Failed to find given verification code");
        
        if (DateTime.UtcNow - existingCode.CreatedAt > TimeSpan.FromSeconds(_configuration.LifespanSeconds))
            throw new BadRequestException("Provided verification code is expired");
        
        UserEntity? user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.PhoneNumber == existingCode.PhoneNumber, ct);
        bool userCreated = false;
        if (user is null)
        {
            user = UserEntityFactory.FromPhoneNumber(existingCode.PhoneNumber);
            _dbContext.Users.Add(user);
            userCreated = true;
        }
        else
        {
            await _dbContext.RefreshTokens.Where(x => x.UserId == user.UserId)
                .ExecuteDeleteAsync(ct);
        }

        await _cacheService.RemoveAsync(cacheKey, ct);
        
        await _dbContext.VerificationCodes.Where(x => x.PhoneNumber == user.PhoneNumber)
            .ExecuteDeleteAsync(ct);
        
        string accessToken = _jwtTokenService.GenerateAccessToken(user);
        string refreshToken = _jwtTokenService.GenerateRefreshToken();
        
        _dbContext.RefreshTokens.Add(new RefreshTokenEntity
        {
            UserId = user.UserId,
            TokenHash = _jwtTokenService.HashToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtTokenService.RefreshTokenLifespanMinutes)
        });
        
        await _dbContext.SaveChangesAsync(ct);

        return new VerifyVerificationCodeResponseDto()
        {
            Tokens = new AccessTokenDto()
            {
                AccessToken = accessToken,
                AccessTokenExpiresInMinutes = _jwtTokenService.AccessTokenLifespanMinutes,
                RefreshToken = refreshToken,
                RefreshTokenExpiresInMinutes = _jwtTokenService.RefreshTokenLifespanMinutes
            },
            CreatedUser = userCreated ? user.MapToDto() : null
        };
    }
}