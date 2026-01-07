using GeoSpot.Application.Services.Interfaces;
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
public record VerifyVerificationCodeRequest(VerifyVerificationCodeRequestDto RequestDto) : IRequest<VerifyVerificationCodeResponseDto>;


public class VerifyVerificationCodeHandler : IRequestHandler<VerifyVerificationCodeRequest, VerifyVerificationCodeResponseDto>
{
    private readonly IVerificationCodeRepository _verificationCodeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly VerificationCodeConfigurationSection _configuration;

    public VerifyVerificationCodeHandler(IVerificationCodeRepository verificationCodeRepository, IUserRepository userRepository, 
        IJwtTokenService jwtTokenService, IRefreshTokenRepository refreshTokenRepository, IOptions<VerificationCodeConfigurationSection> options)
    {
        _verificationCodeRepository = verificationCodeRepository;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _configuration = options.Value;
    }

    public async Task<VerifyVerificationCodeResponseDto> Handle(VerifyVerificationCodeRequest request, CancellationToken ct)
    {
        VerificationCodeModel existingCode = await _verificationCodeRepository.GetVerificationCodeAsync(request.RequestDto.VerificationCodeId, ct)
            ?? throw new NotFoundException($"Failed to find verification code with the given ID. ID = {request.RequestDto.VerificationCodeId}");
        
        if(request.RequestDto.VerificationCode != existingCode.VerificationCode)
            throw new BadRequestException("Provided verification code is invalid");
        
        if (DateTime.UtcNow - existingCode.CreatedAt > TimeSpan.FromSeconds(_configuration.LifespanSeconds))
            throw new BadRequestException("Provided verification code is expired");
        
        UserModel existingUser = await _userRepository.GetUserByPhoneNumberAsync(existingCode.PhoneNumber, ct)
            ?? await _userRepository.CreateUserAsync(CreateUserModel.FromPhoneNumber(existingCode.PhoneNumber) , ct);
        
        string accessToken = _jwtTokenService.GenerateAccessToken(existingUser);
        string refreshToken = _jwtTokenService.GenerateRefreshToken();
        
        await _refreshTokenRepository.DeleteAllUserRefreshTokensAsync(existingUser.UserId, ct);
        
        await _refreshTokenRepository.CreateRefreshTokenAsync(new CreateRefreshTokenModel
        {
            UserId = existingUser.UserId,
            TokenHash = _jwtTokenService.HashToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtTokenService.RefreshTokenLifespanMinutes)
        }, ct);
        
        await _verificationCodeRepository.DeleteVerificationCodeAsync(existingCode.VerificationCodeId, ct);
        
        return new VerifyVerificationCodeResponseDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresInMinutes = _jwtTokenService.AccessTokenLifespanMinutes,
            RefreshToken = refreshToken,
            RefreshTokenExpiresInMinutes = _jwtTokenService.RefreshTokenLifespanMinutes
        };
    }
}