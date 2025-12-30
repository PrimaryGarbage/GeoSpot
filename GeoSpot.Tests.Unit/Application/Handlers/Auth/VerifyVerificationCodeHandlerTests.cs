using GeoSpot.Application.Handlers.Auth;
using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Contracts.Auth;
using GeoSpot.Persistence.Repositories.Interfaces;
using NSubstitute;
using FluentAssertions;
using GeoSpot.Common.Exceptions;
using GeoSpot.Persistence.Repositories.Models.RefreshToken;
using GeoSpot.Persistence.Repositories.Models.User;
using GeoSpot.Persistence.Repositories.Models.VerificationCode;

namespace GeoSpot.Tests.Unit.Application.Handlers.Auth;

public class VerifyVerificationCodeHandlerTests
{
    private readonly VerifyVerificationCodeHandler _handler;
    private readonly IVerificationCodeRepository _verificationCodeRepositoryMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IJwtTokenService _jwtTokenServiceMock;
    private readonly IRefreshTokenRepository _refreshTokenRepositoryMock;

    public VerifyVerificationCodeHandlerTests()
    {
        _verificationCodeRepositoryMock = Substitute.For<IVerificationCodeRepository>();
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _jwtTokenServiceMock = Substitute.For<IJwtTokenService>();
        _refreshTokenRepositoryMock = Substitute.For<IRefreshTokenRepository>();
        
        _handler = new VerifyVerificationCodeHandler(_verificationCodeRepositoryMock, _userRepositoryMock, _jwtTokenServiceMock, 
            _refreshTokenRepositoryMock);
    }
    
    [Fact]
    public async Task Handle_WhenCodeIsValidAndUserDoesntExist_CreatesUserAndReturnsTokens()
    {
        // Arrange
        const int accessTokenLifespan = 10;
        const int refreshTokenLifespan = 30;
        const string phoneNumber = "test_phone_number";
        const string accessToken = "test_access_token";
        const string refreshToken = "test_refresh_token";
        const string hashedRefreshToken = "test_hashed_refresh_token";
        Guid createdUserId = Guid.NewGuid();
        VerifyVerificationCodeRequestDto request = new(Guid.NewGuid(), "test_verification_code");
        CancellationToken ct = CancellationToken.None;
        
        _verificationCodeRepositoryMock.GetVerificationCodeAsync(request.VerificationCodeId, ct)
            .Returns(Task.FromResult<VerificationCodeModel?>(
                new VerificationCodeModel
                {
                   VerificationCodeId = request.VerificationCodeId, 
                   VerificationCode = request.VerificationCode,
                   PhoneNumber = phoneNumber
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
        VerifyVerificationCodeResponseDto result = await _handler.Handle(new VerifyVerificationCodeRequest(request), ct);
        
        // Assert
        await _refreshTokenRepositoryMock.Received().DeleteAllUserRefreshTokensAsync(createdUserId, ct);
        
        await _refreshTokenRepositoryMock.Received().CreateRefreshTokenAsync(Arg.Is<CreateRefreshTokenModel>(
            x => x.UserId == createdUserId && x.TokenHash == hashedRefreshToken), ct);
        
        await _verificationCodeRepositoryMock.Received().DeleteVerificationCodeAsync(request.VerificationCodeId, ct);
        
        result.AccessToken.Should().Be(accessToken);
        result.AccessTokenExpiresInMinutes.Should().Be(accessTokenLifespan);
        result.RefreshToken.Should().Be(refreshToken);
        result.RefreshTokenExpiresInMinutes.Should().Be(refreshTokenLifespan);
    }

    [Fact]
    public async Task Handle_WhenCodeIdIsInvalid_ThrowsNotFoundException()
    {
        // Arrange
        VerifyVerificationCodeRequestDto request = new(Guid.NewGuid(), "test_verification_code");
        CancellationToken ct = CancellationToken.None;
        
        _verificationCodeRepositoryMock.GetVerificationCodeAsync(request.VerificationCodeId, ct)
            .Returns(Task.FromResult<VerificationCodeModel?>(null));
        
        // Act
        var action = async () => await _handler.Handle(new VerifyVerificationCodeRequest(request), ct);
        
        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenCodeDoesntMatch_ThrowsBadRequestException()
    {
        // Arrange
        const string verificationCode = "test_verification_code";
        const string phoneNumber = "test_phone_number";
        VerifyVerificationCodeRequestDto request = new(Guid.NewGuid(), "invalid_verification_code");
        CancellationToken ct = CancellationToken.None;
        
        _verificationCodeRepositoryMock.GetVerificationCodeAsync(request.VerificationCodeId, ct)
            .Returns(Task.FromResult<VerificationCodeModel?>(
                new VerificationCodeModel
                {
                   VerificationCodeId = request.VerificationCodeId, 
                   VerificationCode = verificationCode,
                   PhoneNumber = phoneNumber
                }));
        // Act
        var action = async () => await _handler.Handle(new VerifyVerificationCodeRequest(request), ct);
        
        // Assert
        await action.Should().ThrowAsync<BadRequestException>();
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
        VerifyVerificationCodeRequestDto request = new(Guid.NewGuid(), "test_verification_code");
        CancellationToken ct = CancellationToken.None;
        
        _verificationCodeRepositoryMock.GetVerificationCodeAsync(request.VerificationCodeId, ct)
            .Returns(Task.FromResult<VerificationCodeModel?>(
                new VerificationCodeModel
                {
                   VerificationCodeId = request.VerificationCodeId, 
                   VerificationCode = request.VerificationCode,
                   PhoneNumber = phoneNumber
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
        VerifyVerificationCodeResponseDto result = await _handler.Handle(new VerifyVerificationCodeRequest(request), ct);
        
        // Assert
        await _refreshTokenRepositoryMock.Received().DeleteAllUserRefreshTokensAsync(existingUserId, ct);
        
        await _refreshTokenRepositoryMock.Received().CreateRefreshTokenAsync(Arg.Is<CreateRefreshTokenModel>(
            x => x.UserId == existingUserId && x.TokenHash == hashedRefreshToken), ct);
        
        await _verificationCodeRepositoryMock.Received().DeleteVerificationCodeAsync(request.VerificationCodeId, ct);
        
        result.AccessToken.Should().Be(accessToken);
        result.AccessTokenExpiresInMinutes.Should().Be(accessTokenLifespan);
        result.RefreshToken.Should().Be(refreshToken);
        result.RefreshTokenExpiresInMinutes.Should().Be(refreshTokenLifespan);
    }
}
