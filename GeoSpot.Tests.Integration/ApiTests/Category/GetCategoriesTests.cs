using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Application.Mappers;
using GeoSpot.Contracts.Category;
using static GeoSpot.Tests.Integration.ApiUriPaths;

namespace GeoSpot.Tests.Integration.ApiTests.Category;

public class GetCategoriesTests : ApiIntegrationTestsBase
{
    public GetCategoriesTests(ApiIntegrationFixture fixture) : base(fixture)
    {}

    [Fact]
    public async Task GetCategories_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(CategoriesUri.Categories);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCategories_WhenRequestIsValid_ReturnsCategories()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(CategoriesUri.Categories);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        GetCategoriesResponseDto? response = await responseMessage.Content.ReadFromJsonAsync<GetCategoriesResponseDto>();
        response.Should().NotBeNull();
        response.Categories.Should().BeEquivalentTo(DbContext.Categories.Select(x => x.MapToDto()));
    }
}