using GeoSpot.Application.Mappers.User;
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
public record VerifyVerificationCodeRequest(VerifyVerificationCodeRequestDto Dto) : IRequest<VerifyVerificationCodeResponseDto>;


public class VerifyVerificationCodeHandler : IRequestHandler<VerifyVerificationCodeRequest, VerifyVerificationCodeResponseDto>
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

    public async Task<VerifyVerificationCodeResponseDto> Handle(VerifyVerificationCodeRequest request, CancellationToken ct)
    {
        string cacheKey = CacheKeys.VerificationCodeModel(request.Dto.PhoneNumber);
        
        VerificationCodeModel existingCode = await _cacheService.GetAsync<VerificationCodeModel>(cacheKey, ct)
            ?? await _verificationCodeRepository.GetVerificationCodeAsync(request.Dto.PhoneNumber, request.Dto.VerificationCode, ct)
            ?? throw new NotFoundException("Failed to find given verification code");
        
        if (DateTime.UtcNow - existingCode.CreatedAt > TimeSpan.FromSeconds(_configuration.LifespanSeconds))
            throw new BadRequestException("Provided verification code is expired");

        bool userCreated = false;
        UserModel? user = await _userRepository.GetUserByPhoneNumberAsync(existingCode.PhoneNumber, ct);
        if (user is null)
        {
            user = await _userRepository.CreateUserAsync(CreateUserModel.FromPhoneNumber(existingCode.PhoneNumber) , ct);
            userCreated = true;
        }
        
        string accessToken = _jwtTokenService.GenerateAccessToken(user);
        string refreshToken = _jwtTokenService.GenerateRefreshToken();
        
        await _cacheService.RemoveAsync(cacheKey, ct);
        
        await using (var _ = _unitOfWork.Start())
        {
            await _refreshTokenRepository.DeleteAllUserRefreshTokensAsync(user.UserId, ct);
            await _verificationCodeRepository.DeleteAllUserVerificationCodesAsync(existingCode.PhoneNumber, ct);
        
            await _refreshTokenRepository.CreateRefreshTokenAsync(new CreateRefreshTokenModel
            {
                UserId = user.UserId,
                TokenHash = _jwtTokenService.HashToken(refreshToken),
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtTokenService.RefreshTokenLifespanMinutes)
            }, ct);
        }

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