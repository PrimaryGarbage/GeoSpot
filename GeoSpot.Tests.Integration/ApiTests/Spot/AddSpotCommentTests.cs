using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Tests.Integration.ApiTests.Spot;

public class AddSpotCommentTests : ApiIntegrationTestsBase
{
    public AddSpotCommentTests(ApiIntegrationFixture fixture) : base(fixture)
    {}
    
    [Fact]
    public async Task AddSpotComment_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        AddSpotCommentRequestDto requestDto = new() { Text = "Comment text" };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(ApiUriPaths.SpotsUri.SpotComments(spotId), requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddSpotComment_WhenCurrentUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);
        UserEntity userEntity = (await DbContext.Users.FindAsync(currentUser.UserId))!;
        DbContext.Entry(userEntity).State = EntityState.Deleted;
        AddSpotCommentRequestDto requestDto = new() { Text = "Comment text" };
        
        SpotEntity spot = new()
        {
            CreatorId = currentUser.UserId,
            Title = "Test Spot Title",
        };
        DbContext.Spots.Add(spot);
        
        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(ApiUriPaths.SpotsUri.SpotComments(spot.SpotId), requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddSpotComment_WhenSpotDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);
        AddSpotCommentRequestDto requestDto = new() { Text = "Comment text" };

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(ApiUriPaths.SpotsUri.SpotComments(spotId), requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddSpotComment_WhenTextIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);
        AddSpotCommentRequestDto requestDto = new() { Text = string.Empty };

        SpotEntity spot = new()
        {
            CreatorId = currentUser.UserId,
            Title = "Spot Title",
        };
        DbContext.Spots.Add(spot);

        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(ApiUriPaths.SpotsUri.SpotComments(spot.SpotId), requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddSpotComment_WhenRequestIsValid_ReturnsCommentIdAndOk()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);
        AddSpotCommentRequestDto requestDto = new() { Text = "Comment text" };
        
        SpotEntity spot = new()
        {
            CreatorId = currentUser.UserId,
            Title = "Test Spot Title",
        };
        DbContext.Spots.Add(spot);
        
        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(ApiUriPaths.SpotsUri.SpotComments(spot.SpotId), requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        AddSpotCommentResponseDto? response = await responseMessage.Content.ReadFromJsonAsync<AddSpotCommentResponseDto>();
        response.Should().NotBeNull();
        response.CommentId.Should().Be(DbContext.SpotComments.First(x => x.SpotId == spot.SpotId).SpotCommentId);
    }
}