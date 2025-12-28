using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Contracts.Auth;
using static GeoSpot.Tests.Integration.ApiTests.GeoSpotUriConstants;

namespace GeoSpot.Tests.Integration.ApiTests.Auth;

public class AuthApiTests : IClassFixture<PostgresFixture>
{
    private readonly HttpClient _client;
    
    public AuthApiTests(PostgresFixture fixture)
    {
        GeoSpotWebApplicationFactory factory = new(fixture.ConnectionString);
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task SendCode_WhenRequestIsValid_Returns201()
    {
        const string phoneNumber = "+123456789";
        SendVerificationCodeRequestDto dto = new(phoneNumber);
        
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync(AuthUri.SendVerificationCode, dto);
        
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}