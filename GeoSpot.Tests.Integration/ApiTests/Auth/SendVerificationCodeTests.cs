using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Contracts.Auth;
using static GeoSpot.Tests.Integration.ApiTests.Constants.GeoSpotUriConstants;

namespace GeoSpot.Tests.Integration.ApiTests.Auth;

[Collection("AuthTest")]
public class SendVerificationCodeTests : IClassFixture<PostgresFixture<AuthWebApplicationFactory>>
{
    private readonly HttpClient _client;
    
    public SendVerificationCodeTests(PostgresFixture<AuthWebApplicationFactory> fixture)
    {
        _client = fixture.HttpClient;
    }
    
    [Fact]
    public async Task SendVerificationCode_WhenRequestIsValid_Returns201()
    {
        const string phoneNumber = "+123456789";
        SendVerificationCodeRequestDto dto = new(phoneNumber);
        
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync(AuthUri.SendVerificationCode, dto);
        
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Fact]
    public async Task SendVerificationCode_PhoneNumberIsInvalid_Returns400()
    {
        const string phoneNumber = "";
        SendVerificationCodeRequestDto dto = new(phoneNumber);

        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync(AuthUri.SendVerificationCode, dto);

        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}