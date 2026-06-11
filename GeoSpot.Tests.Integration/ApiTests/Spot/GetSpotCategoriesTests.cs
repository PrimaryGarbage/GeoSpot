using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence.Entities;
using static GeoSpot.Tests.Integration.ApiUriPaths;

namespace GeoSpot.Tests.Integration.ApiTests.Spot;

public class GetSpotCategoriesTests : ApiIntegrationTestsBase
{
    public GetSpotCategoriesTests(ApiIntegrationFixture fixture) : base(fixture)
    {}

    [Fact]
    public async Task GetSpotCategories_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(SpotsUri.SpotCategories(Guid.NewGuid()));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSpotCategories_WhenSpotDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(SpotsUri.SpotCategories(Guid.NewGuid()));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSpotCategories_WhenRequestIsValid_ReturnsSpotCategories()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);
        List<CategoryEntity> categories = await InsertCategories();
        SpotEntity spot = new()
        {
            CreatorId = currentUser.UserId,
            Title = "Spot Title",
            Categories = categories.Take(3).ToList(),
        };
        DbContext.Add(spot);
        
        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(SpotsUri.SpotCategories(spot.SpotId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        GetSpotCategoriesResponseDto? response = await responseMessage.Content.ReadFromJsonAsync<GetSpotCategoriesResponseDto>();
        response.Should().NotBeNull();
        response.Categories.Should().BeEquivalentTo(categories.Take(3));
    }
    
    #region Helpers

    private async Task<List<CategoryEntity>> InsertCategories()
    {
        List<CategoryEntity> categories =
        [
            new CategoryEntity
            {
                Name = "Category 1",
                Color = "Color",
            },
            new CategoryEntity
            {
                Name = "Category 2",
                Color = "Color",
            },
            new CategoryEntity
            {
                Name = "Category 3",
                Color = "Color",
            },
            new CategoryEntity
            {
                Name = "Category 4",
                Color = "Color",
            },
            new CategoryEntity
            {
                Name = "Category 5",
                Color = "Color",
            },
        ];

        DbContext.Categories.AddRange(categories);
        await DbContext.SaveChangesAsync();

        return categories;
    }

    #endregion
}