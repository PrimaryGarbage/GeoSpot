using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Common.Enums;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence.Entities;
using GeoSpot.Tests.Integration.Constants;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Tests.Integration.ApiTests.Spot;

public class UpdateSpotTests : ApiIntegrationTestsBase
{
    public UpdateSpotTests(ApiIntegrationFixture fixture) : base(fixture)
    { }
    
    [Fact] 
    public async Task UpdateSpot_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        
        // Act
        HttpResponseMessage responseMessage = await client.PutAsync(UriConstants.SpotsUri.SpotById(spotId), null);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateSpot_WhenLatitudeIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);
        
        UpdateSpotRequestDto dto = new()
        {
            Title = "Updated Spot Title",
            Description = "Updated Spot Description",
            SpotType = SpotType.Meetup,
            Latitude = 123.0,
            Longitude = 170.0,
            Radius = 5000,
            ImageUrl = "Updated Image Url",
            Address = "Updated Address"
        };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(UriConstants.SpotsUri.SpotById(spotId), dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateSpot_WhenLongitudeIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        UpdateSpotRequestDto dto = new()
        {
            Title = "Updated Spot Title",
            Description = "Updated Spot Description",
            SpotType = SpotType.Meetup,
            Latitude = 88.0,
            Longitude = 256.0,
            Radius = 5000,
            ImageUrl = "Updated Image Url",
            Address = "Updated Address"
        };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(UriConstants.SpotsUri.SpotById(spotId), dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateSpot_WhenRadiusIsNegative_ReturnsBadRequest()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        UpdateSpotRequestDto dto = new()
        {
            Title = "Updated Spot Title",
            Latitude = 88.0,
            Longitude = 175.0,
            Radius = -10,
            Description = "Updated Spot Description",
            SpotType = SpotType.Meetup,
            ImageUrl = "Updated Image Url",
            Address = "Updated Address"
        };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(UriConstants.SpotsUri.SpotById(spotId), dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateSpot_WhenRadiusIsTooBig_ReturnsBadRequest()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        UpdateSpotRequestDto dto = new()
        {
            Title = "Updated Spot Title",
            Latitude = 88.0,
            Longitude = 175.0,
            Radius = int.MaxValue,
            Description = "Updated Spot Description",
            SpotType = SpotType.Meetup,
            ImageUrl = "Updated Image Url",
            Address = "Updated Address"
        };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(UriConstants.SpotsUri.SpotById(spotId), dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateSpot_WhenSpotTypeIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        UpdateSpotRequestDto dto = new()
        {
            Title = "Updated Spot Title",
            Latitude = 88.0,
            Longitude = 175.0,
            Radius = 5000,
            SpotType = (SpotType)Enum.GetValues<SpotType>().Length + 1,
            Description = "Updated Spot Description",
            ImageUrl = "Updated Image Url",
            Address = "Updated Address"
        };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(UriConstants.SpotsUri.SpotById(spotId), dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSpot_WhenCurrentUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);
        UserEntity userEntity = (await DbContext.Users.FindAsync(userActor.UserId))!;
        DbContext.Entry(userEntity).State = EntityState.Deleted;
        await DbContext.SaveChangesAsync();

        UpdateSpotRequestDto dto = new()
        {
            Title = "Updated Spot Title",
            Description = "Updated Spot Description",
            SpotType = SpotType.Meetup,
            Latitude = 88.0,
            Longitude = 175.0,
            Radius = 5000,
            ImageUrl = "Updated Image Url",
            Address = "Updated Address"
        };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(UriConstants.SpotsUri.SpotById(spotId), dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateSpot_WhenSpotIdIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        Guid spotId = Guid.Empty;
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        UpdateSpotRequestDto dto = new()
        {
            Title = "Updated Spot Title",
            Description = "Updated Spot Description",
            SpotType = SpotType.Meetup,
            Latitude = 88.0,
            Longitude = 175.0,
            Radius = 5000,
            ImageUrl = "Updated Image Url",
            Address = "Updated Address"
        };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(UriConstants.SpotsUri.SpotById(spotId), dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSpot_WhenSpotDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        UpdateSpotRequestDto dto = new()
        {
            Title = "Updated Spot Title",
            Description = "Updated Spot Description",
            SpotType = SpotType.Meetup,
            Latitude = 88.0,
            Longitude = 175.0,
            Radius = 5000,
            ImageUrl = "Updated Image Url",
            Address = "Updated Address"
        };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(UriConstants.SpotsUri.SpotById(spotId), dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateSpot_WhenRequestIsValid_ReturnsOk()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);
        
        SpotEntity spot = new()
        {
            Title = "Spot Title",
            Description = "Spot Description",
            CreatorId = userActor.UserId,
            SpotType = SpotType.Event,
            Latitude = 10.0,
            Longitude = 100.0,
            Radius = 3000,
            ImageUrl = "Image Url",
            Address = "Address",
        };
        
        DbContext.Spots.Add(spot);
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();

        UpdateSpotRequestDto dto = new()
        {
            Title = "Updated Spot Title",
            Description = "Updated Spot Description",
            SpotType = SpotType.Meetup,
            Latitude = 88.0,
            Longitude = 175.0,
            Radius = 5000,
            ImageUrl = "Updated Image Url",
            Address = "Updated Address"
        };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(UriConstants.SpotsUri.SpotById(spot.SpotId), dto);
        responseMessage.IsSuccessStatusCode.Should().BeTrue();

        // Assert
        SpotEntity? updatedSpot = await DbContext.Spots.AsNoTracking().FirstOrDefaultAsync(x => x.SpotId == spot.SpotId);
        updatedSpot.Should().NotBeNull();
        updatedSpot.Title.Should().Be(dto.Title);
        updatedSpot.Description.Should().Be(dto.Description);
        updatedSpot.SpotType.Should().Be(dto.SpotType);
        updatedSpot.Latitude.Should().Be(dto.Latitude);
        updatedSpot.Longitude.Should().Be(dto.Longitude);
        updatedSpot.Radius.Should().Be(dto.Radius);
        updatedSpot.ImageUrl.Should().Be(dto.ImageUrl);
        updatedSpot.Address.Should().Be(dto.Address);
    }
}