using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Common.Enums;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using static GeoSpot.Tests.Integration.Constants.UriConstants;

namespace GeoSpot.Tests.Integration.ApiTests.Spot;

public class CreateSpotTests : ApiIntegrationTestsBase
{
    public CreateSpotTests(ApiIntegrationFixture fixture) : base(fixture)
    { }
    
    [Fact] 
    public async Task CreateSpot_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        HttpClient client = CreateClient();
        
        // Act
        HttpResponseMessage responseMessage = await client.PostAsync(SpotsUri.Spots, null);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateSpot_WhenLatitudeIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);
        
        CreateSpotRequestDto dto = new()
        {
            Title = "Spot Title",
            Latitude = 123.0,
            Longitude = 155.0,
            Radius = 5000,
            StartsAt = DateTime.UtcNow.AddDays(1),
            EndsAt = DateTime.UtcNow.AddDays(2)
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(SpotsUri.Spots, dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSpot_WhenLongitudeIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        CreateSpotRequestDto dto = new()
        {
            Title = "Spot Title",
            Latitude = 88.0,
            Longitude = 256.0,
            Radius = 5000,
            StartsAt = DateTime.UtcNow.AddDays(1),
            EndsAt = DateTime.UtcNow.AddDays(2)
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(SpotsUri.Spots, dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSpot_WhenRadiusIsNegative_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        CreateSpotRequestDto dto = new()
        {
            Title = "Spot Title",
            Latitude = 88.0,
            Longitude = 175.0,
            Radius = -10,
            StartsAt = DateTime.UtcNow.AddDays(1),
            EndsAt = DateTime.UtcNow.AddDays(2)
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(SpotsUri.Spots, dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSpot_WhenRadiusIsTooBig_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        CreateSpotRequestDto dto = new()
        {
            Title = "Spot Title",
            Latitude = 88.0,
            Longitude = 175.0,
            Radius = int.MaxValue,
            StartsAt = DateTime.UtcNow.AddDays(1),
            EndsAt = DateTime.UtcNow.AddDays(2)
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(SpotsUri.Spots, dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSpot_WhenSpotTypeIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        CreateSpotRequestDto dto = new()
        {
            Title = "Spot Title",
            Latitude = 88.0,
            Longitude = 175.0,
            Radius = 5000,
            SpotType = (SpotType)Enum.GetValues<SpotType>().Length + 1,
            StartsAt = DateTime.UtcNow.AddDays(1),
            EndsAt = DateTime.UtcNow.AddDays(2)
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(SpotsUri.Spots, dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSpot_WhenStartDateIsLessThanNow_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        CreateSpotRequestDto dto = new()
        {
            Title = "Spot Title",
            Latitude = 88.0,
            Longitude = 175.0,
            Radius = 5000,
            StartsAt = DateTime.UtcNow.AddDays(-1),
            EndsAt = DateTime.UtcNow.AddDays(2)
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(SpotsUri.Spots, dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSpot_WhenEndDateIsTooFarAway_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        CreateSpotRequestDto dto = new()
        {
            Title = "Spot Title",
            Latitude = 88.0,
            Longitude = 175.0,
            Radius = 5000,
            StartsAt = DateTime.UtcNow.AddDays(1),
            EndsAt = DateTime.UtcNow.AddDays(10000)
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(SpotsUri.Spots, dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSpot_WhenGivenBusinessProfileIdIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        CreateSpotRequestDto dto = new()
        {
            Title = "Spot Title",
            Latitude = 88.0,
            Longitude = 175.0,
            Radius = 5000,
            BusinessProfileId = Guid.Empty,
            StartsAt = DateTime.UtcNow.AddDays(1),
            EndsAt = DateTime.UtcNow.AddDays(2)
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(SpotsUri.Spots, dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSpot_WhenGivenBusinessProfileIdDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        CreateSpotRequestDto dto = new()
        {
            Title = "Spot Title",
            Latitude = 88.0,
            Longitude = 175.0,
            Radius = 5000,
            BusinessProfileId = Guid.NewGuid(),
            StartsAt = DateTime.UtcNow.AddDays(1),
            EndsAt = DateTime.UtcNow.AddDays(2)
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(SpotsUri.Spots, dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateSpot_WhenCurrentUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);
        UserEntity userEntity = (await DbContext.Users.FindAsync(userActor.UserId))!;
        DbContext.Entry(userEntity).State = EntityState.Deleted;
        await DbContext.SaveChangesAsync();

        CreateSpotRequestDto dto = new()
        {
            Title = "Spot Title",
            Latitude = 88.0,
            Longitude = 175.0,
            Radius = 5000,
            BusinessProfileId = Guid.NewGuid(),
            StartsAt = DateTime.UtcNow.AddDays(1),
            EndsAt = DateTime.UtcNow.AddDays(2)
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(SpotsUri.Spots, dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateSpot_WhenRequestIsValidAndHasBusinessProfileId_ReturnsCreatedSpot()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);
        
        CategoryEntity categoryEntity = new()
        {
            Name = "Test Category",
            Color = "Color"
        };
        DbContext.Categories.Add(categoryEntity);
        
        BusinessProfileEntity businessProfileEntity = new()
        {
            Name = "Test Business Profile Name",
            UserId = userActor.UserId,
            CategoryId = categoryEntity.CategoryId
        };
        DbContext.BusinessProfiles.Add(businessProfileEntity);
        
        await DbContext.SaveChangesAsync();

        CreateSpotRequestDto dto = new()
        {
            Title = "Spot Title",
            Description = "Spot Description",
            ImageUrl = "Spot Image Url",
            Address = "Spot Address",
            SpotType = SpotType.Meetup,
            Latitude = 88.0,
            Longitude = 175.0,
            Radius = 5000,
            BusinessProfileId = businessProfileEntity.BusinessProfileId,
            StartsAt = DateTime.UtcNow.AddDays(1),
            EndsAt = DateTime.UtcNow.AddDays(2)
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(SpotsUri.Spots, dto);
        responseMessage.IsSuccessStatusCode.Should().BeTrue();
        SpotDto? response = await responseMessage.Content.ReadFromJsonAsync<SpotDto>();

        // Assert
        response.Should().NotBeNull();
        response.SpotId.Should().NotBeEmpty();
        response.BusinessProfileId.Should().Be(dto.BusinessProfileId);
        response.Title.Should().Be(dto.Title);
        response.Description.Should().Be(dto.Description);
        response.ImageUrl.Should().Be(dto.ImageUrl);
        response.Address.Should().Be(dto.Address);
        response.SpotType.Should().Be(dto.SpotType);
        response.Latitude.Should().Be(dto.Latitude);
        response.Longitude.Should().Be(dto.Longitude);
        response.Radius.Should().Be(dto.Radius);
        response.StartsAt.Should().Be(dto.StartsAt);
        response.EndsAt.Should().Be(dto.EndsAt);
    }
    
    [Fact]
    public async Task CreateSpot_WhenRequestIsValidAndDoesNotHaveBusinessProfileId_ReturnsCreatedSpot()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);
        
        CreateSpotRequestDto dto = new()
        {
            Title = "Spot Title",
            Description = "Spot Description",
            ImageUrl = "Spot Image Url",
            Address = "Spot Address",
            SpotType = SpotType.Meetup,
            Latitude = 88.0,
            Longitude = 175.0,
            Radius = 5000,
            StartsAt = DateTime.UtcNow.AddDays(1),
            EndsAt = DateTime.UtcNow.AddDays(2)
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(SpotsUri.Spots, dto);
        responseMessage.IsSuccessStatusCode.Should().BeTrue();
        SpotDto? response = await responseMessage.Content.ReadFromJsonAsync<SpotDto>();

        // Assert
        response.Should().NotBeNull();
        response.SpotId.Should().NotBeEmpty();
        response.BusinessProfileId.Should().BeNull();
        response.Title.Should().Be(dto.Title);
        response.Description.Should().Be(dto.Description);
        response.ImageUrl.Should().Be(dto.ImageUrl);
        response.Address.Should().Be(dto.Address);
        response.SpotType.Should().Be(dto.SpotType);
        response.Latitude.Should().Be(dto.Latitude);
        response.Longitude.Should().Be(dto.Longitude);
        response.Radius.Should().Be(dto.Radius);
        response.StartsAt.Should().Be(dto.StartsAt);
        response.EndsAt.Should().Be(dto.EndsAt);
    }
}