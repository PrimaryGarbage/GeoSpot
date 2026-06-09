using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Common.Enums;
using GeoSpot.Contracts.Device;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using static GeoSpot.Tests.Integration.ApiUriPaths;

namespace GeoSpot.Tests.Integration.ApiTests.Device;

public class RegisterDeviceTests : ApiIntegrationTestsBase
{
    public RegisterDeviceTests(ApiIntegrationFixture fixture) : base(fixture)
    {}
    
    [Fact]
    public async Task RegisterDevice_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        HttpClient client = CreateClient();
        RegisterDeviceRequestDto requestDto = new()
        {
            Token = Guid.NewGuid().ToString(),
            Platform = Platform.Android,
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(DevicesUri.Register, requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegisterDevice_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);
        DbContext.Entry(currentUser).State = EntityState.Deleted;
        await DbContext.SaveChangesAsync();
        
        RegisterDeviceRequestDto requestDto = new()
        {
            Token = Guid.NewGuid().ToString(),
            Platform = Platform.Android,
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(DevicesUri.Register, requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task? RegisterDevice_WhenDeviceIsAlreadyRegistered_ReturnsBadRequest()
    {
        // Arrange
        string deviceToken = Guid.NewGuid().ToString();
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);
        
        DeviceTokenEntity deviceTokenEntity = new()
        {
            Token = deviceToken,
            UserId = currentUser.UserId,
            IsActive = true,
        };
        
        DbContext.DeviceTokens.Add(deviceTokenEntity);
        await DbContext.SaveChangesAsync();

        RegisterDeviceRequestDto requestDto = new()
        {
            Token = deviceToken,
            Platform = Platform.Android,
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(DevicesUri.Register, requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterDevice_WhenRequestIsValid_CreatesDeviceTokenAndReturnsTokenId()
    {
        // Arrange
        string deviceToken = Guid.NewGuid().ToString();
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);

        RegisterDeviceRequestDto requestDto = new()
        {
            Token = deviceToken,
            Platform = Platform.Android,
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(DevicesUri.Register, requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        RegisterDeviceResponseDto? response = await responseMessage.Content.ReadFromJsonAsync<RegisterDeviceResponseDto>();
        response.Should().NotBeNull();
        DeviceTokenEntity? deviceTokenEntity = await DbContext.DeviceTokens.AsNoTracking().FirstOrDefaultAsync(x => 
            x.UserId == currentUser.UserId && x.Token == deviceToken);
        deviceTokenEntity.Should().NotBeNull();
        deviceTokenEntity.IsActive.Should().BeTrue();
        response.DeviceTokenId.Should().Be(deviceTokenEntity.DeviceTokenId);
    }
}