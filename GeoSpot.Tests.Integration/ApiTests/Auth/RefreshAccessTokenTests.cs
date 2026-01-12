using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Application.Services.Interfaces;
using GeoSpot.Contracts.Auth;
using GeoSpot.Persistence.Entities;
using GeoSpot.Persistence.Entities.Factories;
using GeoSpot.Tests.Integration.Constants;
using Microsoft.Extensions.DependencyInjection;

namespace GeoSpot.Tests.Integration.ApiTests.Auth;

public class RefreshAccessTokenTests : ApiIntegrationTestsBase, IClassFixture<ApiIntegrationFixture>
{
    private readonly IJwtTokenService _jwtTokenService;
    
    public RefreshAccessTokenTests(ApiIntegrationFixture fixture) : base(fixture)
    {
        _jwtTokenService = fixture.Services.GetRequiredService<IJwtTokenService>();
    }
    
    [Fact]
    public async Task RefreshAccessToken_WhenRefreshTokenIsInvalid_ReturnsNotFound()
    {
        // Arrange
        const string invalidRefreshToken = "invalid_refresh_token";
        RefreshAccessTokenRequestDto requestDto = new(invalidRefreshToken);
        HttpClient client = CreateClient();
 
        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(UriConstants.Auth.RefreshAccessToken, requestDto);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RefreshAccessToken_WhenRefreshTokenIsRevoked_ReturnsBadRequest()
    {
        // Arrange
        const string refreshToken = "invalid_refresh_token";
        const string phoneNumber = "+123456789";
        RefreshAccessTokenRequestDto requestDto = new(refreshToken);
        HttpClient client = CreateClient();
 
        UserEntity existingUser = DbContext.Users.Add(UserEntityFactory.FromPhoneNumber(phoneNumber)).Entity;
        
        DbContext.RefreshTokens.Add(new RefreshTokenEntity
        {
            TokenHash = _jwtTokenService.HashToken(refreshToken),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow + TimeSpan.FromDays(1),
            UserId = existingUser.UserId,
            Revoked = true
        });
        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage =
            await client.PostAsJsonAsync(UriConstants.Auth.RefreshAccessToken, requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshAccessToken_WhenRefreshTokenIsExpired_ReturnsBadRequest()
    {
        // Arrange
        const string refreshToken = "invalid_refresh_token";
        const string phoneNumber = "+123456789";
        RefreshAccessTokenRequestDto requestDto = new(refreshToken);
        HttpClient client = CreateClient();

        UserEntity existingUser =
            DbContext.Users.Add(UserEntityFactory.FromPhoneNumber(phoneNumber)).Entity;

        DbContext.RefreshTokens.Add(new RefreshTokenEntity
        {
            TokenHash = _jwtTokenService.HashToken(refreshToken),
            CreatedAt = DateTime.UtcNow - TimeSpan.FromDays(1),
            ExpiresAt = DateTime.UtcNow - TimeSpan.FromHours(1),
            UserId = existingUser.UserId,
        });
        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage =
            await client.PostAsJsonAsync(UriConstants.Auth.RefreshAccessToken, requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshAccessToken_WhenRefreshTokenIsValid_ReturnsNewRefreshAndAccessTokens()
    {
        // Arrange
        const string refreshToken = "invalid_refresh_token";
        const string phoneNumber = "+123456789";
        RefreshAccessTokenRequestDto requestDto = new(refreshToken);
        HttpClient client = CreateClient();

        UserEntity existingUser =
            DbContext.Users.Add(UserEntityFactory.FromPhoneNumber(phoneNumber)).Entity;

        DbContext.RefreshTokens.Add(new RefreshTokenEntity
        {
            TokenHash = _jwtTokenService.HashToken(refreshToken),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow + TimeSpan.FromDays(1),
            UserId = existingUser.UserId,
        });
        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage =
            await client.PostAsJsonAsync(UriConstants.Auth.RefreshAccessToken, requestDto);
        responseMessage.IsSuccessStatusCode.Should().BeTrue();
        
        AccessTokenDto? response = await responseMessage.Content.ReadFromJsonAsync<AccessTokenDto>();
        
        // Assert
        response.Should().NotBeNull();
        response.AccessToken.Should().NotBeEmpty();
        response.AccessTokenExpiresInMinutes.Should().BeGreaterThan(0);
        response.RefreshToken.Should().NotBeEmpty();
        response.RefreshToken.Should().NotBe(refreshToken);
        response.RefreshTokenExpiresInMinutes.Should().BeGreaterThan(0);
    }
}