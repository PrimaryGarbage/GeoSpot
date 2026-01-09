using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Common;
using GeoSpot.Common.ConfigurationSections;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Auth;
using GeoSpot.Persistence.Repositories.Interfaces;
using GeoSpot.Persistence.Repositories.Models.RefreshToken;
using GeoSpot.Persistence.Repositories.Models.User;
using GeoSpot.Persistence.Repositories.Models.VerificationCode;
using Microsoft.Extensions.Options;

namespace GeoSpot.Application.Dispatcher.Handlers.Auth;

[ExcludeFromCodeCoverage]
public record VerifyVerificationCodeRequest(VerifyVerificationCodeRequestDto Dto) : IRequest<AccessTokenDto>;


public class VerifyVerificationCodeHandler : IRequestHandler<VerifyVerificationCodeRequest, AccessTokenDto>
{
    private readonly IVerificationCodeRepository _verificationCodeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly VerificationCodeConfigurationSection _configuration;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyVerificationCodeHandler(IVerificationCodeRepository verificationCodeRepository, IUserRepository userRepository, 
        IJwtTokenService jwtTokenService, IRefreshTokenRepository refreshTokenRepository, IOptions<VerificationCodeConfigurationSection> options, 
        ICacheService cacheService, IUnitOfWork unitOfWork)
    {
        _verificationCodeRepository = verificationCodeRepository;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _configuration = options.Value;
    }

    public async Task<AccessTokenDto> Handle(VerifyVerificationCodeRequest request, CancellationToken ct)
    {
        string cacheKey = CacheKeys.VerificationCodeModel(request.Dto.PhoneNumber);
        
        VerificationCodeModel existingCode = await _cacheService.GetAsync<VerificationCodeModel>(cacheKey, ct)
            ?? await _verificationCodeRepository.GetVerificationCodeAsync(request.Dto.PhoneNumber, request.Dto.VerificationCode, ct)
            ?? throw new NotFoundException("Failed to find given verification code");
        
        if (DateTime.UtcNow - existingCode.CreatedAt > TimeSpan.FromSeconds(_configuration.LifespanSeconds))
            throw new BadRequestException("Provided verification code is expired");
        
        UserModel existingUser = await _userRepository.GetUserByPhoneNumberAsync(existingCode.PhoneNumber, ct)
            ?? await _userRepository.CreateUserAsync(CreateUserModel.FromPhoneNumber(existingCode.PhoneNumber) , ct);
        
        string accessToken = _jwtTokenService.GenerateAccessToken(existingUser);
        string refreshToken = _jwtTokenService.GenerateRefreshToken();
        
        await _cacheService.RemoveAsync(cacheKey, ct);
        
        await using (var scope = _unitOfWork.Start())
        {
            await _refreshTokenRepository.DeleteAllUserRefreshTokensAsync(existingUser.UserId, ct);
            await _verificationCodeRepository.DeleteAllUserVerificationCodesAsync(existingCode.PhoneNumber, ct);
        
            await _refreshTokenRepository.CreateRefreshTokenAsync(new CreateRefreshTokenModel
            {
                UserId = existingUser.UserId,
                TokenHash = _jwtTokenService.HashToken(refreshToken),
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtTokenService.RefreshTokenLifespanMinutes)
            }, ct);
        }

        
        return new AccessTokenDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresInMinutes = _jwtTokenService.AccessTokenLifespanMinutes,
            RefreshToken = refreshToken,
            RefreshTokenExpiresInMinutes = _jwtTokenService.RefreshTokenLifespanMinutes
        };
    }
}