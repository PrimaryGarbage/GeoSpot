using System.Net;
using FluentAssertions;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Tests.Integration.ApiTests.Spot;

public class RemoveSpotCommentTests : ApiIntegrationTestsBase
{
    public RemoveSpotCommentTests(ApiIntegrationFixture fixture) : base(fixture)
    {}

    [Fact]
    public async Task RemoveSpotComment_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        Guid commentId = Guid.NewGuid();
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage responseMessage = await client.DeleteAsync(ApiUriPaths.SpotsUri.SpotComment(commentId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RemoveSpotComment_WhenCurrentUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);
        UserEntity userEntity = (await DbContext.Users.FindAsync(currentUser.UserId))!;
        DbContext.Entry(userEntity).State = EntityState.Deleted;

        SpotEntity spot = new()
        {
            CreatorId = currentUser.UserId,
            Title = "Test Spot Title",
        };
        DbContext.Spots.Add(spot);
        
        SpotCommentEntity comment = new()
        {
            SpotId = spot.SpotId,
            CreatorId = currentUser.UserId,
            Text = "Test Spot Comment Text",
        };
        DbContext.SpotComments.Add(comment);

        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage = await client.DeleteAsync(ApiUriPaths.SpotsUri.SpotComment(comment.SpotCommentId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveSpotComment_WhenCommentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        Guid commentId = Guid.NewGuid();
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);

        SpotEntity spot = new()
        {
            CreatorId = currentUser.UserId,
            Title = "Test Spot Title",
        };
        DbContext.Spots.Add(spot);

        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage = await client.DeleteAsync(ApiUriPaths.SpotsUri.SpotComment(commentId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveSpotComment_WhenRequestIsValid_RemovesCommentAndReturnsNoContent()
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

        SpotCommentEntity comment = new()
        {
            SpotId = spot.SpotId,
            CreatorId = currentUser.UserId,
            Text = "Test Spot Comment Text",
        };
        DbContext.SpotComments.Add(comment);

        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage = await client.DeleteAsync(ApiUriPaths.SpotsUri.SpotComment(comment.SpotCommentId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);
        DbContext.SpotComments.FirstOrDefault(x => x.SpotId == spot.SpotId).Should().BeNull();
    }
}