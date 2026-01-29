using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Contracts.User;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using static GeoSpot.Tests.Integration.ApiUriPaths;

namespace GeoSpot.Tests.Integration.ApiTests.User;

public class GetCurrentUserCategoriesTests : ApiIntegrationTestsBase
{
    public GetCurrentUserCategoriesTests(ApiIntegrationFixture fixture) : base(fixture)
    { }
    
    [Fact] 
    public async Task GetCurrentUserCategories_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        HttpClient client = CreateClient();
        
        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(UsersUri.CurrentUserCategories);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    
    [Fact]
    public async Task GetCurrentUserCategories_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);
        UserEntity userEntity = (await DbContext.Users.FindAsync(currentUser.UserId))!;
        DbContext.Entry(userEntity).State = EntityState.Deleted;
        await DbContext.SaveChangesAsync();
        
        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(UsersUri.CurrentUserCategories);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCurrentUserCategories_WhenUserDoesNotHaveCategories_ReturnsEmptyCollection()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(UsersUri.CurrentUserCategories);
        GetCurrentUserCategoriesResponseDto? response =
            await responseMessage.Content.ReadFromJsonAsync<GetCurrentUserCategoriesResponseDto>();

        // Assert
        response.Should().NotBeNull();
        response.Categories.Should().HaveCount(0);
    }

    [Fact]
    public async Task GetCurrentUserCategories_WhenUserHasCategories_ReturnsCategories()
    {
        // Arrange
        const int categoryCount = 3;
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);
        
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
        ];
        
        await DbContext.Attach(currentUser).Collection(x => x.Categories!).LoadAsync();
        
        currentUser.Categories.AddRange(categories);
        await DbContext.SaveChangesAsync();
        
        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(UsersUri.CurrentUserCategories);
        GetCurrentUserCategoriesResponseDto? response = await responseMessage.Content.ReadFromJsonAsync<GetCurrentUserCategoriesResponseDto>();

        // Assert
        response.Should().NotBeNull();
        response.Categories.Should().HaveCount(categoryCount);
        response.Categories.Select(x => x.CategoryId).Should().BeEquivalentTo(categories.Select(x => x.CategoryId));
    }
}