using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Contracts.Auth.v1;
using GeoSpot.Persistence.Entities;
using GeoSpot.Tests.Integration.Constants;

namespace GeoSpot.Tests.Integration.ApiTests.Auth;

public class VerifyVerificationCodeTests : ApiIntegrationTestsBase, IClassFixture<ApiIntegrationFixture>
{
    public VerifyVerificationCodeTests(ApiIntegrationFixture fixture) : base(fixture)
    {}
    
    [Fact]
    public async Task VerifyVerificationCode_WhenCodeLengthIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        const string phoneNumber = "+123456789";
        const string invalidVerificationCode = "test_invalid_verification_code";
        VerifyVerificationCodeRequestDto requestDto = new(phoneNumber, invalidVerificationCode);
        CancellationToken ct = CancellationToken.None;
        HttpClient client = CreateClient();
        
        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(UriConstants.Auth.VerifyVerificationCode, requestDto, ct);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task VerifyVerificationCode_WhenCodeIsExpired_ReturnsBadRequest()
    {
        // Arrange
        const string phoneNumber = "+123456789";
        const string existingVerificationCode = "456789";
        CancellationToken ct = CancellationToken.None;
        HttpClient client = CreateClient();

        VerificationCodeEntity existingCodeEntity = new()
        {
            PhoneNumber = phoneNumber,
            VerificationCode = existingVerificationCode,
        };

        DbContext.VerificationCodes.Add(existingCodeEntity);
        await DbContext.SaveChangesAsync(ct);
        existingCodeEntity.CreatedAt = DateTime.UtcNow.AddHours(-3);
        await DbContext.SaveChangesAsync(ct);

        VerifyVerificationCodeRequestDto requestDto = new(phoneNumber, existingVerificationCode);

        // Act
        HttpResponseMessage responseMessage =
            await client.PostAsJsonAsync(UriConstants.Auth.VerifyVerificationCode, requestDto, ct);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task VerifyVerificationCode_WhenCodeIsInvalid_ReturnsNotFound()
    {
        // Arrange
        const string phoneNumber = "+123456789";
        const string invalidVerificationCode = "123456";
        const string existingVerificationCode = "456789";
        CancellationToken ct = CancellationToken.None;
        HttpClient client = CreateClient();
        
        VerificationCodeEntity existingCodeEntity = new()
        {
            PhoneNumber = phoneNumber,
            VerificationCode = existingVerificationCode,
        };
        
        DbContext.VerificationCodes.Add(existingCodeEntity);
        await DbContext.SaveChangesAsync(ct);
        
        VerifyVerificationCodeRequestDto requestDto = new(phoneNumber, invalidVerificationCode);

        // Act
        HttpResponseMessage responseMessage =
            await client.PostAsJsonAsync(UriConstants.Auth.VerifyVerificationCode, requestDto, ct);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task VerifyVerificationCode_WhenCodeExistsAndUserDoesNotExist_ReturnsNewUserAndTokens()
    {
        // Arrange
        const string phoneNumber = "+123456788";
        const string existingVerificationCode = "456788";
        CancellationToken ct = CancellationToken.None;
        HttpClient client = CreateClient();

        VerificationCodeEntity existingCodeEntity = new()
        {
            PhoneNumber = phoneNumber,
            VerificationCode = existingVerificationCode,
        };

        DbContext.VerificationCodes.Add(existingCodeEntity);   // Cleanup isn't required because code is deleted by handler  
        await DbContext.SaveChangesAsync(ct);   
        
        VerifyVerificationCodeRequestDto requestDto = new(phoneNumber, existingVerificationCode);

        // Act
        HttpResponseMessage responseMessage =
            await client.PostAsJsonAsync(UriConstants.Auth.VerifyVerificationCode, requestDto, ct);
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        VerifyVerificationCodeResponseDto? response = 
            await responseMessage.Content.ReadFromJsonAsync<VerifyVerificationCodeResponseDto>(ct);

        // Assert
        response.Should().NotBeNull();
        response.Tokens.AccessToken.Should().NotBeEmpty();
        response.Tokens.AccessTokenExpiresInMinutes.Should().BeGreaterThan(0);
        response.Tokens.RefreshToken.Should().NotBeEmpty();
        response.Tokens.RefreshTokenExpiresInMinutes.Should().BeGreaterThan(0);
        
        response.CreatedUser.Should().NotBeNull();
        response.CreatedUser.PhoneNumber.Should().Be(phoneNumber);
    }

    [Fact]
    public async Task VerifyVerificationCode_WhenCodeExistsAndUserExist_ReturnsAccessAndRefreshTokens()
    {
        // Arrange
        const string phoneNumber = "+123456789";
        const string displayName = "test_display_name";
        const string existingVerificationCode = "456789";
        CancellationToken ct = CancellationToken.None;
        HttpClient client = CreateClient();

        VerificationCodeEntity existingCodeEntity = new()
        {
            PhoneNumber = phoneNumber,
            VerificationCode = existingVerificationCode,
        };
        
        UserEntity existingUser = new()
        {
            PhoneNumber = phoneNumber,
            DisplayName = displayName
        };

        DbContext.VerificationCodes.Add(existingCodeEntity);   // Cleanup isn't required because code is deleted by handler  
        DbContext.Users.Add(existingUser);
        await DbContext.SaveChangesAsync(ct);
        
        VerifyVerificationCodeRequestDto requestDto = new(phoneNumber, existingVerificationCode);

        // Act
        HttpResponseMessage responseMessage =
            await client.PostAsJsonAsync(UriConstants.Auth.VerifyVerificationCode, requestDto, ct);
        VerifyVerificationCodeResponseDto? response =
            await responseMessage.Content.ReadFromJsonAsync<VerifyVerificationCodeResponseDto>(ct);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
        response.Tokens.AccessToken.Should().NotBeEmpty();
        response.Tokens.AccessTokenExpiresInMinutes.Should().BeGreaterThan(0);
        response.Tokens.RefreshToken.Should().NotBeEmpty();
        response.Tokens.RefreshTokenExpiresInMinutes.Should().BeGreaterThan(0);
        
        response.CreatedUser.Should().BeNull();
    }
}