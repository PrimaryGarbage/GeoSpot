using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Contracts.Auth;
using GeoSpot.Tests.Integration.Constants;

namespace GeoSpot.Tests.Integration.ApiTests.Auth;

public class SendVerificationCodeTests : ApiIntegrationTestsBase, IClassFixture<ApiIntegrationFixture>
{
    public SendVerificationCodeTests(ApiIntegrationFixture fixture) : base(fixture)
    {}
    
    [Fact]
    public async Task SendVerificationCode_WhenRequestIsValid_Returns201()
    {
        const string phoneNumber = "+123456789";
        SendVerificationCodeRequestDto dto = new(phoneNumber);
        HttpClient client = CreateClient();
        
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(UriConstants.Auth.SendVerificationCode, dto);
        
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Fact]
    public async Task SendVerificationCode_PhoneNumberIsInvalid_Returns400()
    {
        const string phoneNumber = "";
        SendVerificationCodeRequestDto dto = new(phoneNumber);
        HttpClient client = CreateClient();

        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(UriConstants.Auth.SendVerificationCode, dto);

        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}