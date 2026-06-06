using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Common.Enums;
using GeoSpot.Contracts.Device;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using static GeoSpot.Tests.Integration.ApiUriPaths;

namespace GeoSpot.Tests.Integration.ApiTests.Device;

public class UnregisterDeviceTests : ApiIntegrationTestsBase
{
    public UnregisterDeviceTests(ApiIntegrationFixture fixture) : base(fixture)
    {}

    [Fact]
    public async Task UnregisterDevice_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        Guid deviceTokenId = Guid.NewGuid();
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage responseMessage = await client.DeleteAsync(DevicesUri.Unregister(deviceTokenId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UnregisterDevice_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        Guid deviceTokenId = Guid.NewGuid();
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);
        DbContext.Entry(currentUser).State = EntityState.Deleted;
        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage = await client.DeleteAsync(DevicesUri.Unregister(deviceTokenId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UnregisterDevice_WhenUserDeviceIsNotRegistered_ReturnsNotFound()
    {
        // Arrange
        Guid deviceTokenId = Guid.NewGuid();
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        // Act
        HttpResponseMessage responseMessage = await client.DeleteAsync(DevicesUri.Unregister(deviceTokenId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UnregisterDevice_WhenRequestIsValid_ReturnsNoContent()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);
        
        DeviceTokenEntity deviceToken = new()
        {
            UserId = currentUser.UserId,
            Token = Guid.NewGuid().ToString(),
            Platform = Platform.Android,
            IsActive = true,
        };
        
        DbContext.DeviceTokens.Add(deviceToken);
        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage = await client.DeleteAsync(DevicesUri.Unregister(deviceToken.DeviceTokenId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);
        DeviceTokenEntity? deletedDeviceToken = await DbContext.DeviceTokens.AsNoTracking().FirstOrDefaultAsync(x => x.DeviceTokenId == deviceToken.DeviceTokenId);
        deletedDeviceToken.Should().BeNull();
    }
}