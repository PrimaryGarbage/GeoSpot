using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Contracts.Auth;
using GeoSpot.Tests.Integration.Constants;
using static GeoSpot.Tests.Integration.Constants.GeoSpotUriConstants;

namespace GeoSpot.Tests.Integration.ApiTests.Auth;

[Collection(CollectionConstants.ApiIntegrationCollectionName)]
public class SendVerificationCodeTests : ApiIntegrationTestsBase, IClassFixture<ApiIntegrationFixture>
{
    public SendVerificationCodeTests(ApiIntegrationFixture fixture) : base(fixture.HttpClient, fixture.DbContext)
    {}
    
    [Fact]
    public async Task SendVerificationCode_WhenRequestIsValid_Returns201()
    {
        const string phoneNumber = "+123456789";
        SendVerificationCodeRequestDto dto = new(phoneNumber);
        
        HttpResponseMessage responseMessage = await Client.PostAsJsonAsync(AuthUri.SendVerificationCode, dto);
        
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Fact]
    public async Task SendVerificationCode_PhoneNumberIsInvalid_Returns400()
    {
        const string phoneNumber = "";
        SendVerificationCodeRequestDto dto = new(phoneNumber);

        HttpResponseMessage responseMessage = await Client.PostAsJsonAsync(AuthUri.SendVerificationCode, dto);

        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}