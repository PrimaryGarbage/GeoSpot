using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Contracts.User;
using GeoSpot.Persistence.Entities;
using GeoSpot.Tests.Integration.Constants;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Tests.Integration.ApiTests.Users;

public class UpdateCurrentUserLocationTests : ApiIntegrationTestsBase
{
    public UpdateCurrentUserLocationTests(ApiIntegrationFixture fixture) : base(fixture)
    { }

    [Fact] 
    public async Task UpdateCurrentUserLocation_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        HttpClient client = CreateClient();
        
        // Act
        HttpResponseMessage responseMessage = await client.PutAsync(UriConstants.Users.UpdateCurrentUserLocation, null);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task UpdateCurrentUserLocation_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        const double updatedLatitude = 45.0;
        const double updatedLongitude = 122.0;
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);
        
        UserEntity userEntity = (await DbContext.Users.FindAsync(userActor.UserId))!;
        DbContext.Entry(userEntity).State = EntityState.Deleted;
        await DbContext.SaveChangesAsync();
        
        UpdateCurrentUserLocationRequestDto requestDto = new()
        {
            Latitude = updatedLatitude,
            Longitude = updatedLongitude
        };
        
        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(UriConstants.Users.UpdateCurrentUserLocation, requestDto);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCurrentUserLocation_WhenRequestLatitudeIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        const double updatedInvalidLatitude = -130.0;
        const double updatedLongitude = 122.0;
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        UpdateCurrentUserLocationRequestDto requestDto = new()
        {
            Latitude = updatedInvalidLatitude,
            Longitude = updatedLongitude
        };

        // Act
        HttpResponseMessage responseMessage =
            await client.PutAsJsonAsync(UriConstants.Users.UpdateCurrentUserLocation, requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCurrentUserLocation_WhenRequestLongitudeIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        const double updatedLatitude = -45.0;
        const double updatedInvalidLongitude = 235.0;
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        UpdateCurrentUserLocationRequestDto requestDto = new()
        {
            Latitude = updatedLatitude,
            Longitude = updatedInvalidLongitude
        };

        // Act
        HttpResponseMessage responseMessage =
            await client.PutAsJsonAsync(UriConstants.Users.UpdateCurrentUserLocation, requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCurrentUserLocation_WhenRequestIsValid_UpdatesUserLocation()
    {
        // Arrange
        const double updatedLatitude = -45.0;
        const double updatedLongitude = 135.56;
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);

        UpdateCurrentUserLocationRequestDto requestDto = new()
        {
            Latitude = updatedLatitude,
            Longitude = updatedLongitude
        };

        // Act
        HttpResponseMessage responseMessage =
            await client.PutAsJsonAsync(UriConstants.Users.UpdateCurrentUserLocation, requestDto);

        // Assert
        responseMessage.IsSuccessStatusCode.Should().BeTrue();
        UserEntity updatedUser = await DbContext.Users.FirstAsync(x => x.UserId == userActor.UserId);
        updatedUser.LastLatitude.Should().Be(updatedLatitude);
        updatedUser.LastLongitude.Should().Be(updatedLongitude);
    }
}