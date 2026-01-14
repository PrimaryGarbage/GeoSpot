using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence.Entities;
using GeoSpot.Tests.Integration.Constants;

namespace GeoSpot.Tests.Integration.ApiTests.Spot;

public class SearchNearbySpotsTests : ApiIntegrationTestsBase
{
    public SearchNearbySpotsTests(ApiIntegrationFixture fixture) : base(fixture)
    { }
    
    [Fact] 
    public async Task SearchNearbySpots_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        HttpClient client = CreateClient();
        
        // Act
        HttpResponseMessage responseMessage = await client.PostAsync(UriConstants.Spots.SearchNearbySpots, null);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SearchNearbySpots_WhenLatitudeIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);
        
        SearchNearbySpotsRequestDto dto = new()
        {
            Latitude = 123.0,
            Longitude = 155.0,
            Radius = 5000
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(UriConstants.Spots.SearchNearbySpots, dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchNearbySpots_WhenLongitudeIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);

        SearchNearbySpotsRequestDto dto = new()
        {
            Latitude = 56.0,
            Longitude = 234.0,
            Radius = 5000
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(UriConstants.Spots.SearchNearbySpots, dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchNearbySpots_WhenRadiusIsNegative_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);

        SearchNearbySpotsRequestDto dto = new()
        {
            Latitude = 56.0,
            Longitude = 174.0,
            Radius = -5000
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(UriConstants.Spots.SearchNearbySpots, dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchNearbySpots_WhenRadiusIsTooBig_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);

        SearchNearbySpotsRequestDto dto = new()
        {
            Latitude = 56.0,
            Longitude = 174.0,
            Radius = int.MaxValue
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(UriConstants.Spots.SearchNearbySpots, dto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task SearchNearbySpots_WhenNoNearbySpots_ReturnsEmptyCollection()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);
        
        SearchNearbySpotsRequestDto dto = new()
        {
            Latitude = 56.0,
            Longitude = 174.0,
            Radius = 500
        };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(UriConstants.Spots.SearchNearbySpots, dto);
        var test = await responseMessage.Content.ReadAsStringAsync();
        responseMessage.IsSuccessStatusCode.Should().BeTrue();
        SearchNearbySpotsResponseDto? response = await responseMessage.Content.ReadFromJsonAsync<SearchNearbySpotsResponseDto>();

        // Assert
        response.Should().NotBeNull();
        response.Spots.Should().NotBeNull();
        response.Spots.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchNearbySpots_WhenFoundNearbySpots_ReturnsSpots()
    {
        // Arrange
        const double userLatitude = 1.0;
        const double userLongitude = 1.0;
        const int userToSpotDistance = 500;
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);
        
        List<SpotEntity> spots = [
            new SpotEntity
            {
                Title = "Spot 1",
                Latitude = userLatitude + MetersToLatitudeDelta(userToSpotDistance - 1),
                Longitude = userLongitude,
                CreatorId = userActor.UserId
            },
            new SpotEntity
            {
                Title = "Spot 2",
                Latitude = userLatitude,
                Longitude = userLongitude + MetersToLongitudeDelta(userToSpotDistance -1, userLatitude),
                CreatorId = userActor.UserId
            },
            new SpotEntity
            {
                Title = "Spot 3",
                CreatorId = userActor.UserId
            }
        ];
        
        SearchNearbySpotsRequestDto dto = new()
        {
            Latitude = userLatitude,
            Longitude = userLongitude,
            Radius = userToSpotDistance
        };
        
        DbContext.Spots.AddRange(spots);
        await DbContext.SaveChangesAsync();
        
        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(UriConstants.Spots.SearchNearbySpots, dto);
        responseMessage.IsSuccessStatusCode.Should().BeTrue();
        SearchNearbySpotsResponseDto? response = await responseMessage.Content.ReadFromJsonAsync<SearchNearbySpotsResponseDto>();

        // Assert
        response.Should().NotBeNull();
        response.Spots.Should().NotBeNull();
        response.Spots.Should().HaveCount(2);
    }
    
    private static double MetersToLatitudeDelta(int meters)
    {
        return meters * 0.0000089;
    }

    private static double MetersToLongitudeDelta(int meters, double latitude)
    {
        return meters * 0.0000089 / Math.Cos(Math.PI * latitude / 180.0);
    }
}