using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Tests.Integration.ApiTests.Spot;

public class GetSpotCommentsTests : ApiIntegrationTestsBase
{
    public GetSpotCommentsTests(ApiIntegrationFixture fixture) : base(fixture)
    {}
    
    [Fact]
    public async Task GetSpotComments_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(ApiUriPaths.SpotsUri.SpotComments(spotId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task GetSpotComments_WhenSpotDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(ApiUriPaths.SpotsUri.SpotComments(spotId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSpotComments_WhenRequestIsValid_ReturnsComments()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);

        SpotEntity spot = new()
        {
            CreatorId = currentUser.UserId,
            Title = "Test Spot Title",
        };
        DbContext.Spots.Add(spot);

        SpotCommentEntity comment1 = new()
        {
            CreatorId = currentUser.UserId,
            SpotId = spot.SpotId,
            Text = "Comment Text 1",
        };
        SpotCommentEntity comment2 = new()
        {
            CreatorId = currentUser.UserId,
            SpotId = spot.SpotId,
            Text = "Comment Text 2",
        };
        DbContext.SpotComments.Add(comment1);
        DbContext.SpotComments.Add(comment2);

        await DbContext.SaveChangesAsync();
        
        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(ApiUriPaths.SpotsUri.SpotComments(spot.SpotId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        GetSpotCommentsResponseDto? comments = await responseMessage.Content.ReadFromJsonAsync<GetSpotCommentsResponseDto>();
        comments.Should().NotBeNull();
        comments.Comments.Should().HaveCount(2);
        comments.Comments.Should().Contain(x => x.CommentId == comment1.SpotCommentId);
        comments.Comments.Should().Contain(x => x.CommentId == comment2.SpotCommentId);
    }
}