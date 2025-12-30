using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Contracts.Auth;
using GeoSpot.Tests.Integration.ApiTests.Constants;

namespace GeoSpot.Tests.Integration.ApiTests.Auth;

[Collection("AuthTest")]
public class VerifyVerificationCodeTests : IClassFixture<PostgresFixture<AuthWebApplicationFactory>>
{
    private readonly HttpClient _client;

    public VerifyVerificationCodeTests(PostgresFixture<AuthWebApplicationFactory> fixture)
    {
        _client = fixture.HttpClient;
    }
    
    [Fact]
    public async Task VerifyVerificationCode_WhenTokenIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        const string invalidVerificationCode = "test_invalid_verification_code";
        Guid invalidVerificationCodeId = Guid.NewGuid();
        VerifyVerificationCodeRequestDto requestDto = new(invalidVerificationCodeId, invalidVerificationCode);
        CancellationToken ct = CancellationToken.None;
        
        // Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync(GeoSpotUriConstants.AuthUri.VerifyVerificationCode, requestDto, ct);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}