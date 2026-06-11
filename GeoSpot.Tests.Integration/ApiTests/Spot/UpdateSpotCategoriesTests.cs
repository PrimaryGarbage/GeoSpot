using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using static GeoSpot.Tests.Integration.ApiUriPaths;

namespace GeoSpot.Tests.Integration.ApiTests.Spot;

public class UpdateSpotCategoriesTests : ApiIntegrationTestsBase
{
    public UpdateSpotCategoriesTests(ApiIntegrationFixture fixture) : base(fixture)
    {}

    [Fact]
    public async Task UpdateSpotCategories_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage responseMessage = await client.PutAsync(SpotsUri.SpotCategories(Guid.NewGuid()), null);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateSpotCategories_WhenSpotDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);
        UpdateSpotCategoriesRequestDto request = new(new List<Guid>());

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(SpotsUri.SpotCategories(Guid.NewGuid()), request);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateSpotCategories_WhenCategoriesAreEmpty_DeletesSpotCategories()
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
        DbContext.Spots.Add(spot);
        
        await DbContext.SaveChangesAsync();
        
        UpdateSpotCategoriesRequestDto request = new(new List<Guid>());
        
        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(SpotsUri.SpotCategories(spot.SpotId), request);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        SpotEntity updatedSpot = await DbContext.Spots
                                    .AsNoTracking()
                                    .Include(x => x.Categories)
                                    .FirstAsync(x => x.SpotId == spot.SpotId);
        updatedSpot.Categories.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateSpotCategories_WhenAtLeastOneCategoryIdIsInvalid_ReturnNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);
        List<CategoryEntity> categories = await InsertCategories();
        SpotEntity spot = new()
        {
            CreatorId = currentUser.UserId,
            Title = "Spot Title",
            Categories = categories.Take(3).ToList()
        };
        DbContext.Spots.Add(spot);
        
        await DbContext.SaveChangesAsync();
        
        UpdateSpotCategoriesRequestDto request = new([categories.First().CategoryId, Guid.NewGuid()]);

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(SpotsUri.SpotCategories(spot.SpotId), request);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateSpotCategories_WhenRequestIsValid_UpdatesCategories()
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
        DbContext.Spots.Add(spot);
        
        await DbContext.SaveChangesAsync();
        
        UpdateSpotCategoriesRequestDto request = new(categories.TakeLast(2).Select(x => x.CategoryId).ToList());

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(SpotsUri.SpotCategories(spot.SpotId), request);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        SpotEntity updatedSpot = await DbContext.Spots
                                    .AsNoTracking()
                                    .Include(x => x.Categories)
                                    .FirstAsync(x => x.SpotId == spot.SpotId);
        updatedSpot.Categories.Should().BeEquivalentTo(categories.TakeLast(2));
    }
    
    #region Helpers

    private async Task<List<CategoryEntity>> InsertCategories()
    {
        List<CategoryEntity> categories = [
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
