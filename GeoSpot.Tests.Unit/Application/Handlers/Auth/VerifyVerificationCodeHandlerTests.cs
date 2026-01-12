using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Persistence.Repositories.Interfaces;
using NSubstitute;
using FluentAssertions;
using GeoSpot.Application.Dispatcher.Handlers.Auth.v1;
using GeoSpot.Common.ConfigurationSections;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Auth.v1;
using GeoSpot.Persistence.Repositories.Models.RefreshToken;
using GeoSpot.Persistence.Repositories.Models.User;
using GeoSpot.Persistence.Repositories.Models.VerificationCode;
using Microsoft.Extensions.Options;

namespace GeoSpot.Tests.Unit.Application.Handlers.Auth;

public class VerifyVerificationCodeHandlerTests
{
    private readonly VerifyVerificationCodeHandler _handler;
    private readonly IVerificationCodeRepository _verificationCodeRepositoryMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IJwtTokenService _jwtTokenServiceMock;
    private readonly IRefreshTokenRepository _refreshTokenRepositoryMock;
    private readonly VerificationCodeConfigurationSection _configuration;
    private readonly ICacheService _cacheServiceMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public VerifyVerificationCodeHandlerTests()
    {
        _verificationCodeRepositoryMock = Substitute.For<IVerificationCodeRepository>();
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _jwtTokenServiceMock = Substitute.For<IJwtTokenService>();
        _refreshTokenRepositoryMock = Substitute.For<IRefreshTokenRepository>();
        _configuration = new VerificationCodeConfigurationSection
        {
            LifespanSeconds = 300,
            NumberOfDigits = 6
        };
        _cacheServiceMock = Substitute.For<ICacheService>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        
        var optionsMock = Substitute.For<IOptions<VerificationCodeConfigurationSection>>();
        optionsMock.Value.Returns(_configuration);
        
        _handler = new VerifyVerificationCodeHandler(_verificationCodeRepositoryMock, _userRepositoryMock, _jwtTokenServiceMock, 
            _refreshTokenRepositoryMock, optionsMock, _cacheServiceMock, _unitOfWorkMock);
    }
    
    [Fact]
    public async Task Handle_WhenCodeIsValidAndUserDoesntExist_CreatesUserAndReturnsUserAndTokens()
    {
        // Arrange
        const int accessTokenLifespan = 10;
        const int refreshTokenLifespan = 30;
        const string phoneNumber = "test_phone_number";
        const string accessToken = "test_access_token";
        const string refreshToken = "test_refresh_token";
        const string hashedRefreshToken = "test_hashed_refresh_token";
        const string verificationCode = "test_verification_code";
        VerifyVerificationCodeRequestDto requestDto = new(phoneNumber, verificationCode);
        Guid createdUserId = Guid.NewGuid();
        Guid verificationCodeId = Guid.NewGuid();
        CancellationToken ct = CancellationToken.None;
        
        _verificationCodeRepositoryMock.GetVerificationCodeAsync(phoneNumber, verificationCode, ct)
            .Returns(Task.FromResult<VerificationCodeModel?>(
                new VerificationCodeModel
                {
                   VerificationCodeId = verificationCodeId, 
                   VerificationCode = verificationCode,
                   PhoneNumber = phoneNumber,
                   CreatedAt = DateTime.UtcNow
                }));
        
        _userRepositoryMock.GetUserByPhoneNumberAsync(phoneNumber, ct)
            .Returns(Task.FromResult<UserModel?>(null));
        _userRepositoryMock.CreateUserAsync(Arg.Any<CreateUserModel>(), ct)
            .Returns(Task.FromResult(new UserModel { UserId = createdUserId, PhoneNumber = phoneNumber, DisplayName = phoneNumber}));
        
        _jwtTokenServiceMock.AccessTokenLifespanMinutes.Returns(accessTokenLifespan);
        _jwtTokenServiceMock.RefreshTokenLifespanMinutes.Returns(refreshTokenLifespan);
        _jwtTokenServiceMock.GenerateAccessToken(Arg.Any<UserModel>()).Returns(accessToken);
        _jwtTokenServiceMock.GenerateRefreshToken().Returns(refreshToken);
        _jwtTokenServiceMock.HashToken(refreshToken).Returns(hashedRefreshToken);
        
        // Act
        VerifyVerificationCodeResponseDto result = await _handler.Handle(new VerifyVerificationCodeRequest(requestDto), ct);
        
        // Assert
        await _refreshTokenRepositoryMock.Received().DeleteAllUserRefreshTokensAsync(createdUserId, ct);
        
        await _refreshTokenRepositoryMock.Received().CreateRefreshTokenAsync(Arg.Is<CreateRefreshTokenModel>(
            x => x.UserId == createdUserId && x.TokenHash == hashedRefreshToken), ct);
        
        await _verificationCodeRepositoryMock.Received().DeleteAllUserVerificationCodesAsync(phoneNumber, ct);
        
        result.Tokens.AccessToken.Should().Be(accessToken);
        result.Tokens.AccessTokenExpiresInMinutes.Should().Be(accessTokenLifespan);
        result.Tokens.RefreshToken.Should().Be(refreshToken);
        result.Tokens.RefreshTokenExpiresInMinutes.Should().Be(refreshTokenLifespan);
        
        result.CreatedUser.Should().NotBeNull();
        result.CreatedUser.PhoneNumber.Should().Be(phoneNumber);
    }
    
    [Fact]
    public async Task Handle_WhenCodeIsExpired_ThrowsBadRequestException()
    {
        // Arrange
        const string phoneNumber = "test_phone_number";
        const string verificationCode = "test_verification_code";
        VerifyVerificationCodeRequestDto requestDto = new(phoneNumber, verificationCode);
        Guid verificationCodeId = Guid.NewGuid();
        CancellationToken ct = CancellationToken.None;
        
        _verificationCodeRepositoryMock.GetVerificationCodeAsync(phoneNumber, verificationCode, ct)
            .Returns(Task.FromResult<VerificationCodeModel?>(
                new VerificationCodeModel
                {
                   VerificationCodeId = verificationCodeId, 
                   VerificationCode = verificationCode,
                   PhoneNumber = phoneNumber,
                   CreatedAt = DateTime.UtcNow.AddSeconds(-(_configuration.LifespanSeconds + 1))
                }));
        
        // Act
        var action = async () => await _handler.Handle(new VerifyVerificationCodeRequest(requestDto), ct);
        
        // Assert
        await action.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task Handle_WhenCodeIdIsInvalid_ThrowsNotFoundException()
    {
        // Arrange
        const string verificationCode = "test_verification_code";
        const string phoneNumber = "test_phone_number";
        VerifyVerificationCodeRequestDto requestDto = new(phoneNumber, verificationCode);
        CancellationToken ct = CancellationToken.None;
        
        _verificationCodeRepositoryMock.GetVerificationCodeAsync(phoneNumber, verificationCode, ct)
            .Returns(Task.FromResult<VerificationCodeModel?>(null));
        
        // Act
        var action = async () => await _handler.Handle(new VerifyVerificationCodeRequest(requestDto), ct);
        
        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenCodeDoesntMatch_ThrowsBadRequestException()
    {
        // Arrange
        const string phoneNumber = "test_phone_number";
        const string verificationCode = "test_verification_code";
        VerifyVerificationCodeRequestDto requestDto = new(phoneNumber, "invalid_verification_code");
        Guid verificationCodeId = Guid.NewGuid();
        CancellationToken ct = CancellationToken.None;
        
        _verificationCodeRepositoryMock.GetVerificationCodeAsync(phoneNumber, verificationCode, ct)
            .Returns(Task.FromResult<VerificationCodeModel?>(
                new VerificationCodeModel
                {
                   VerificationCodeId = verificationCodeId, 
                   VerificationCode = verificationCode,
                   PhoneNumber = phoneNumber,
                   CreatedAt = DateTime.UtcNow
                }));
        
        // Act
        var action = async () => await _handler.Handle(new VerifyVerificationCodeRequest(requestDto), ct);
        
        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsTokensWithoutCreatingNewUser()
    {
        // Arrange
        const int accessTokenLifespan = 10;
        const int refreshTokenLifespan = 30;
        const string phoneNumber = "test_phone_number";
        const string accessToken = "test_access_token";
        const string refreshToken = "test_refresh_token";
        const string hashedRefreshToken = "test_hashed_refresh_token";
        Guid existingUserId = Guid.NewGuid();
        const string verificationCode = "test_verification_code";
        Guid verificationCodeId = Guid.NewGuid();
        VerifyVerificationCodeRequestDto requestDto = new(phoneNumber, verificationCode);
        CancellationToken ct = CancellationToken.None;
        
        _verificationCodeRepositoryMock.GetVerificationCodeAsync(phoneNumber, verificationCode, ct)
            .Returns(Task.FromResult<VerificationCodeModel?>(
                new VerificationCodeModel
                {
                   VerificationCodeId = verificationCodeId, 
                   VerificationCode = verificationCode,
                   PhoneNumber = phoneNumber,
                   CreatedAt = DateTime.UtcNow
                }));
        
        _userRepositoryMock.GetUserByPhoneNumberAsync(phoneNumber, ct)
            .Returns(Task.FromResult<UserModel?>(
                new UserModel { UserId = existingUserId, PhoneNumber = phoneNumber, DisplayName = phoneNumber}));
        
        _jwtTokenServiceMock.AccessTokenLifespanMinutes.Returns(accessTokenLifespan);
        _jwtTokenServiceMock.RefreshTokenLifespanMinutes.Returns(refreshTokenLifespan);
        _jwtTokenServiceMock.GenerateAccessToken(Arg.Any<UserModel>()).Returns(accessToken);
        _jwtTokenServiceMock.GenerateRefreshToken().Returns(refreshToken);
        _jwtTokenServiceMock.HashToken(refreshToken).Returns(hashedRefreshToken);
        
        // Act
        VerifyVerificationCodeResponseDto result = await _handler.Handle(new VerifyVerificationCodeRequest(requestDto), ct);
        
        // Assert
        await _refreshTokenRepositoryMock.Received().DeleteAllUserRefreshTokensAsync(existingUserId, ct);
        
        await _refreshTokenRepositoryMock.Received().CreateRefreshTokenAsync(Arg.Is<CreateRefreshTokenModel>(
            x => x.UserId == existingUserId && x.TokenHash == hashedRefreshToken), ct);
        
        await _verificationCodeRepositoryMock.Received().DeleteAllUserVerificationCodesAsync(phoneNumber, ct);
        
        result.Tokens.AccessToken.Should().Be(accessToken);
        result.Tokens.AccessTokenExpiresInMinutes.Should().Be(accessTokenLifespan);
        result.Tokens.RefreshToken.Should().Be(refreshToken);
        result.Tokens.RefreshTokenExpiresInMinutes.Should().Be(refreshTokenLifespan);
        
        result.CreatedUser.Should().BeNull();
    }
}
