using System.Net;
using FluentAssertions;
using GeoSpot.Common.Enums;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using static GeoSpot.Tests.Integration.ApiUriPaths;

namespace GeoSpot.Tests.Integration.ApiTests.Spot;

public class AddSpotViewTests : ApiIntegrationTestsBase
{
    public AddSpotViewTests(ApiIntegrationFixture fixture) : base(fixture)
    { }
    
    [Fact] 
    public async Task AddSpotView_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        
        // Act
        HttpResponseMessage responseMessage = await client.PutAsync(SpotsUri.SpotView(spotId), null);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddSpotView_WhenCurrentUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);
        UserEntity userEntity = (await DbContext.Users.FindAsync(currentUser.UserId))!;
        DbContext.Entry(userEntity).State = EntityState.Deleted;
        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage = await client.PutAsync(SpotsUri.SpotView(spotId), null);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddSpotView_WhenSpotIdIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        Guid spotId = Guid.Empty;
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        // Act
        HttpResponseMessage responseMessage = await client.PutAsync(SpotsUri.SpotView(spotId), null);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddSpotView_WhenSpotDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        // Act
        HttpResponseMessage responseMessage = await client.PutAsync(SpotsUri.SpotView(spotId), null);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddSpotView_WhenViewAlreadyExists_ReturnsNoContent()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);

        SpotEntity spot = new()
        {
            Title = "Spot Title",
            Description = "Spot Description",
            CreatorId = currentUser.UserId,
            SpotType = SpotType.Event,
            Latitude = 10.0,
            Longitude = 100.0,
            Radius = 3000,
            ImageUrl = "Image Url",
            Address = "Address",
        };
        DbContext.Spots.Add(spot);
        
        UserSpotViewEntity userSpotView = new()
        {
            UserId = currentUser.UserId,
            SpotId = spot.SpotId
        };
        DbContext.UserSpotViews.Add(userSpotView);
        
        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage = await client.PutAsync(SpotsUri.SpotView(spot.SpotId), null);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task AddSpotView_WhenRequestIsValid_ReturnsNoContent()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);
        
        SpotEntity spot = new()
        {
            Title = "Spot Title",
            Description = "Spot Description",
            CreatorId = currentUser.UserId,
            SpotType = SpotType.Event,
            Latitude = 10.0,
            Longitude = 100.0,
            Radius = 3000,
            ImageUrl = "Image Url",
            Address = "Address",
        };
        
        DbContext.Spots.Add(spot);
        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage = await client.PutAsync(SpotsUri.SpotView(spot.SpotId), null);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);
        UserSpotViewEntity? view = await DbContext.UserSpotViews
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == currentUser.UserId && x.SpotId == spot.SpotId);
        view.Should().NotBeNull();
    }
}