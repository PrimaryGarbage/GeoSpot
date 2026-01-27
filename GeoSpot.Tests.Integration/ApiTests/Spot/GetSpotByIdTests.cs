using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence.Entities;
using GeoSpot.Tests.Integration.Constants;

namespace GeoSpot.Tests.Integration.ApiTests.Spot;

public class GetSpotByIdTests : ApiIntegrationTestsBase
{
    public GetSpotByIdTests(ApiIntegrationFixture fixture) : base(fixture)
    { }

    [Fact]
    public async Task GetSpotById_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(UriConstants.Spots.SpotById(spotId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSpotById_WhenIdIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        Guid spotId = Guid.Empty;
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(UriConstants.Spots.SpotById(spotId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetSpotById_WhenSpotDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(UriConstants.Spots.SpotById(spotId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSpotById_WhenSpotExists_ReturnsCorrectSpot()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);
        
        SpotEntity spotEntity = new()
        {
            Title = "Test Spot Title",
            CreatorId = userActor.UserId
        };
        
        DbContext.Spots.Add(spotEntity);
        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(UriConstants.Spots.SpotById(spotEntity.SpotId));
        responseMessage.IsSuccessStatusCode.Should().BeTrue();
        SpotDto? response = await responseMessage.Content.ReadFromJsonAsync<SpotDto>();

        // Assert
        response.Should().NotBeNull();
        response.SpotId.Should().Be(spotEntity.SpotId);
        response.Title.Should().Be(spotEntity.Title);
    }
}