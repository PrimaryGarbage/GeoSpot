using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Application.Mappers;
using GeoSpot.Contracts.Category;
using GeoSpot.Contracts.User;
using GeoSpot.Persistence.Entities;
using GeoSpot.Tests.Integration.Constants;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace GeoSpot.Tests.Integration.ApiTests.User;

public class UpdateCurrentUserCategoriesTests : ApiIntegrationTestsBase
{
    public UpdateCurrentUserCategoriesTests(ApiIntegrationFixture fixture) : base(fixture)
    { }
    
    [Fact] 
    public async Task UpdateCurrentUserCategories_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        HttpClient client = CreateClient();
        
        // Act
        HttpResponseMessage responseMessage = await client.PutAsync(UriConstants.Users.UpdateCurrentUserCategories, null);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task UpdateCurrentUserCategories_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);
        
        UserEntity userEntity = (await DbContext.Users.FindAsync(userActor.UserId))!;
        DbContext.Entry(userEntity).State = EntityState.Deleted;
        await DbContext.SaveChangesAsync();
        
        UpdateCurrentUserCategoriesRequestDto requestDto = new() { Categories = [] };
        
        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(UriConstants.Users.UpdateCurrentUserCategories, requestDto);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCurrentUserCategories_WhenAllRequestCategoriesDoNotExist_ReturnsNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        List<CategoryEntity> existingCategories =
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
        ];

        DbContext.Categories.AddRange(existingCategories);
        await DbContext.SaveChangesAsync();

        UpdateCurrentUserCategoriesRequestDto requestDto = new()
        {
            Categories =
            [
                new CategoryDto
                {
                    CategoryId = Guid.NewGuid(),
                    Name = "invalidName",
                    Color = "Color"
                }
            ]
        };

        // Act
        HttpResponseMessage responseMessage =
            await client.PutAsJsonAsync(UriConstants.Users.UpdateCurrentUserCategories, requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCurrentUserCategories_WhenSomeRequestCategoriesDoNotExist_ReturnsNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        List<CategoryEntity> existingCategories =
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
        ];

        DbContext.Categories.AddRange(existingCategories);
        await DbContext.SaveChangesAsync();

        UpdateCurrentUserCategoriesRequestDto requestDto = new()
        {
            Categories = existingCategories.Take(2).Select(x => x.MapToDto()).Append(
                new CategoryDto
                {
                    CategoryId = Guid.NewGuid(),
                    Name = "invalidName",
                    Color = "Color"
                })
        };

        // Act
        HttpResponseMessage responseMessage =
            await client.PutAsJsonAsync(UriConstants.Users.UpdateCurrentUserCategories, requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCurrentUserCategories_WhenRequestCategoryHasInvalidId_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        UpdateCurrentUserCategoriesRequestDto requestDto = new()
        {
            Categories = [ new CategoryDto
            {
                CategoryId = Guid.Empty, 
                Name = "invalidName", 
                Color = "Color"
            } ]
        };

        // Act
        HttpResponseMessage responseMessage =
            await client.PutAsJsonAsync(UriConstants.Users.UpdateCurrentUserCategories, requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCurrentUserCategories_WhenRequestCategoriesAreValid_UpdatesUserCategories()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);

        List<CategoryEntity> existingCategories =
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
        ];

        DbContext.Categories.AddRange(existingCategories);
        await DbContext.SaveChangesAsync();

        UpdateCurrentUserCategoriesRequestDto requestDto = new()
        {
            Categories = existingCategories.Select(x => x.MapToDto())
        };

        // Act
        HttpResponseMessage responseMessage =
            await client.PutAsJsonAsync(UriConstants.Users.UpdateCurrentUserCategories, requestDto);

        // Assert
        responseMessage.IsSuccessStatusCode.Should().BeTrue();
        UserEntity existingUser = await DbContext.Users
            .Include(x => x.Categories)
            .AsNoTracking()
            .FirstAsync(x => x.UserId == userActor.UserId);
        existingUser.Categories.Should().HaveCount(existingCategories.Count);
        existingUser.Categories.Select(x => x.CategoryId).Should().BeEquivalentTo(existingCategories.Select(x => x.CategoryId));
    }

    [Fact]
    public async Task UpdateCurrentUserCategories_WhenRequestHasNoCategories_RemovesUserCategories()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);

        List<CategoryEntity> existingCategories =
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
        ];

        await DbContext.Attach(userActor).Collection(x => x.Categories!).LoadAsync();
        userActor.Categories.AddRange(existingCategories);
        await DbContext.SaveChangesAsync();

        UpdateCurrentUserCategoriesRequestDto requestDto = new() { Categories = [] };

        // Act
        HttpResponseMessage responseMessage =
            await client.PutAsJsonAsync(UriConstants.Users.UpdateCurrentUserCategories, requestDto);

        // Assert
        responseMessage.IsSuccessStatusCode.Should().BeTrue();
        UserEntity existingUser = await DbContext.Users
            .Include(x => x.Categories)
            .AsNoTracking()
            .FirstAsync(x => x.UserId == userActor.UserId);
        existingUser.Categories.Should().BeEmpty();
    }
}