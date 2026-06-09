using System.Net;
using FluentAssertions;
using GeoSpot.Common.Enums;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Tests.Integration.ApiTests.Spot;

public class RemoveSpotReactionTests : ApiIntegrationTestsBase
{
    public RemoveSpotReactionTests(ApiIntegrationFixture fixture) : base(fixture)
    {}
    
    [Fact]
    public async Task RemoveSpotReaction_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage responseMessage = await client.DeleteAsync(ApiUriPaths.SpotsUri.SpotReaction(spotId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RemoveSpotReaction_WhenSpotIdIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        Guid spotId = Guid.Empty;
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);

        // Act
        HttpResponseMessage responseMessage = await client.DeleteAsync(ApiUriPaths.SpotsUri.SpotReaction(spotId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemoveSpotReaction_WhenReactionDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);

        SpotEntity spot = new()
        {
            Title = "Spot Title",
            Description = "Spot Description",
            CreatorId = currentUser.UserId,
            SpotType = SpotType.Event,
            Latitude = 10.0,
            Longitude = 100.0,
            Radius = 3000,
            ImageUrl = "Image Url",
            Address = "Address",
        };
        DbContext.Spots.Add(spot);

        ReactionTypeEntity reactionType = new()
        {
            Name = "Name",
            Emoji = "Emoji",
        };
        DbContext.ReactionTypes.Add(reactionType);

        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage = await client.DeleteAsync(ApiUriPaths.SpotsUri.SpotReaction(spot.SpotId));

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveSpotReaction_WhenReactionExists_RemovesReactionAndReturnsNoContent()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);

        SpotEntity spot = new()
        {
            Title = "Spot Title",
            Description = "Spot Description",
            CreatorId = currentUser.UserId,
            SpotType = SpotType.Event,
            Latitude = 10.0,
            Longitude = 100.0,
            Radius = 3000,
            ImageUrl = "Image Url",
            Address = "Address",
        };
        DbContext.Spots.Add(spot);
        
        ReactionTypeEntity reactionType = new()
        {
            Name = "Old Name",
            Emoji = "Old Emoji",
        };
        DbContext.ReactionTypes.Add(reactionType);
        
        SpotReactionEntity spotReaction = new()
        {
            SpotId = spot.SpotId,
            CreatorId = currentUser.UserId,
            ReactionTypeId = reactionType.ReactionTypeId,
        };
        DbContext.SpotReactions.Add(spotReaction);
        
        await DbContext.SaveChangesAsync();
        
        // Act
        HttpResponseMessage responseMessage = await client.DeleteAsync(ApiUriPaths.SpotsUri.SpotReaction(spot.SpotId));

        // Assert
        SpotReactionEntity? resultReaction = await DbContext.SpotReactions.AsNoTracking().FirstOrDefaultAsync(
            x => x.CreatorId == currentUser.UserId && x.SpotId == spot.SpotId);
        resultReaction.Should().BeNull();
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}