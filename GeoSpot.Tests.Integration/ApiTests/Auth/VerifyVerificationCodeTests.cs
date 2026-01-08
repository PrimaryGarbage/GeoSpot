using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Contracts.Auth;
using GeoSpot.Persistence.Entities;
using GeoSpot.Tests.Integration.Constants;

namespace GeoSpot.Tests.Integration.ApiTests.Auth;

[Collection(CollectionConstants.ApiIntegrationCollectionName)]
public class VerifyVerificationCodeTests : ApiIntegrationTestsBase, IClassFixture<ApiIntegrationFixture>
{
    public VerifyVerificationCodeTests(ApiIntegrationFixture fixture) : base(fixture.HttpClient, fixture.DbContext)
    {}
    
    [Fact]
    public async Task VerifyVerificationCode_WhenCodeLengthIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        const string invalidVerificationCode = "test_invalid_verification_code";
        VerifyVerificationCodeRequestDto requestDto = new(invalidVerificationCode);
        CancellationToken ct = CancellationToken.None;
        
        // Act
        HttpResponseMessage responseMessage = await Client.PostAsJsonAsync(GeoSpotUriConstants.AuthUri.VerifyVerificationCode, requestDto, ct);
        
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

        VerificationCodeEntity existingCodeEntity = new()
        {
            PhoneNumber = phoneNumber,
            VerificationCode = existingVerificationCode,
        };

        DbContext.VerificationCodes.Add(existingCodeEntity);
        await DbContext.SaveChangesAsync(ct);
        existingCodeEntity.CreatedAt = DateTime.UtcNow.AddHours(-3);
        await DbContext.SaveChangesAsync(ct);
        AddToCleanup(ctx => ctx.VerificationCodes.Remove(existingCodeEntity));

        VerifyVerificationCodeRequestDto requestDto = new(existingVerificationCode);

        // Act
        HttpResponseMessage responseMessage =
            await Client.PostAsJsonAsync(GeoSpotUriConstants.AuthUri.VerifyVerificationCode, requestDto, ct);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await Cleanup();
    }

    [Fact]
    public async Task VerifyVerificationCode_WhenCodeIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        const string phoneNumber = "+123456789";
        const string invalidVerificationCode = "123456";
        const string existingVerificationCode = "456789";
        CancellationToken ct = CancellationToken.None;
        
        VerificationCodeEntity existingCodeEntity = new()
        {
            PhoneNumber = phoneNumber,
            VerificationCode = existingVerificationCode,
        };
        
        DbContext.VerificationCodes.Add(existingCodeEntity);
        await DbContext.SaveChangesAsync(ct);
        AddToCleanup(ctx => ctx.VerificationCodes.Remove(existingCodeEntity));
        
        VerifyVerificationCodeRequestDto requestDto = new(invalidVerificationCode);

        // Act
        HttpResponseMessage responseMessage =
            await Client.PostAsJsonAsync(GeoSpotUriConstants.AuthUri.VerifyVerificationCode, requestDto, ct);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        await Cleanup();
    }

    [Fact]
    public async Task VerifyVerificationCode_WhenCodeExistsAndUserDoesNotExist_ReturnsAccessAndRefreshTokens()
    {
        // Arrange
        const string phoneNumber = "+123456789";
        const string existingVerificationCode = "456789";
        CancellationToken ct = CancellationToken.None;

        VerificationCodeEntity existingCodeEntity = new()
        {
            PhoneNumber = phoneNumber,
            VerificationCode = existingVerificationCode,
        };

        DbContext.VerificationCodes.Add(existingCodeEntity);   // Cleanup isn't required because code is deleted by handler  
        await DbContext.SaveChangesAsync(ct);   
        
        VerifyVerificationCodeRequestDto requestDto = new(existingVerificationCode);

        // Act
        HttpResponseMessage responseMessage =
            await Client.PostAsJsonAsync(GeoSpotUriConstants.AuthUri.VerifyVerificationCode, requestDto, ct);
        AccessTokenDto? response = 
            await responseMessage.Content.ReadFromJsonAsync<AccessTokenDto>(ct);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
        response.AccessToken.Should().NotBeEmpty();
        response.AccessTokenExpiresInMinutes.Should().BeGreaterThan(0);
        response.RefreshToken.Should().NotBeEmpty();
        response.RefreshTokenExpiresInMinutes.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task VerifyVerificationCode_WhenCodeExistsAndUserExist_ReturnsAccessAndRefreshTokens()
    {
        // Arrange
        const string phoneNumber = "+123456789";
        const string displayName = "test_display_name";
        const string existingVerificationCode = "456789";
        CancellationToken ct = CancellationToken.None;

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
        AddToCleanup(ctx => ctx.Users.Remove(existingUser));
        
        VerifyVerificationCodeRequestDto requestDto = new(existingVerificationCode);

        // Act
        HttpResponseMessage responseMessage =
            await Client.PostAsJsonAsync(GeoSpotUriConstants.AuthUri.VerifyVerificationCode, requestDto, ct);
        AccessTokenDto? response =
            await responseMessage.Content.ReadFromJsonAsync<AccessTokenDto>(ct);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
        response.AccessToken.Should().NotBeEmpty();
        response.AccessTokenExpiresInMinutes.Should().BeGreaterThan(0);
        response.RefreshToken.Should().NotBeEmpty();
        response.RefreshTokenExpiresInMinutes.Should().BeGreaterThan(0);
        
        await Cleanup();
    }
}