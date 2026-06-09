using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Common.Enums;
using GeoSpot.Contracts.Spot;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using static GeoSpot.Tests.Integration.ApiUriPaths;

namespace GeoSpot.Tests.Integration.ApiTests.Spot;

public class AddSpotReactionTests : ApiIntegrationTestsBase
{
    public AddSpotReactionTests(ApiIntegrationFixture fixture) : base(fixture) 
    {}

    [Fact]
    public async Task AddSpotReaction_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        AddSpotReactionRequestDto requestDto = new() { ReactionTypeId = Guid.NewGuid() };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(SpotsUri.SpotReaction(spotId), requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddSpotReaction_WhenCurrentUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);
        UserEntity userEntity = (await DbContext.Users.FindAsync(currentUser.UserId))!;
        DbContext.Entry(userEntity).State = EntityState.Deleted;
        await DbContext.SaveChangesAsync();
        AddSpotReactionRequestDto requestDto = new() { ReactionTypeId = Guid.NewGuid() };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(SpotsUri.SpotReaction(spotId), requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddSpotReaction_WhenSpotIdIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        Guid spotId = Guid.Empty;
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);
        AddSpotReactionRequestDto requestDto = new() { ReactionTypeId = Guid.NewGuid() };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(SpotsUri.SpotReaction(spotId), requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddSpotReaction_WhenReactionTypeIdIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        Guid spotId = Guid.Empty;
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);
        AddSpotReactionRequestDto requestDto = new() { ReactionTypeId = Guid.Empty };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(SpotsUri.SpotReaction(spotId), requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddSpotReaction_WhenReactionTypeDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        Guid spotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);
        AddSpotReactionRequestDto requestDto = new() { ReactionTypeId = Guid.NewGuid() };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(SpotsUri.SpotReaction(spotId), requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddSpotReaction_WhenReactionAlreadyExists_UpdatesReactionTypeAndReturnsNoContent()
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
        
        ReactionTypeEntity oldReactionType = new()
        {
            Name = "Old Name",
            Emoji = "Old Emoji",
        };
        ReactionTypeEntity newReactionType = new()
        {
            Name = "New Name",
            Emoji = "New Emoji",
        };
        DbContext.ReactionTypes.Add(oldReactionType);
        DbContext.ReactionTypes.Add(newReactionType);
        
        SpotReactionEntity spotReaction = new()
        {
            SpotId = spot.SpotId,
            CreatorId = currentUser.UserId,
            ReactionTypeId = oldReactionType.ReactionTypeId,
        };
        DbContext.SpotReactions.Add(spotReaction);
        
        await DbContext.SaveChangesAsync();
        
        AddSpotReactionRequestDto requestDto = new() { ReactionTypeId = newReactionType.ReactionTypeId };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(SpotsUri.SpotReaction(spot.SpotId), requestDto);

        // Assert
        SpotReactionEntity? updatedSpotReaction = await DbContext.SpotReactions.AsNoTracking().FirstOrDefaultAsync(x => x.CreatorId == currentUser.UserId);
        updatedSpotReaction.Should().NotBeNull();
        updatedSpotReaction.ReactionTypeId.Should().Be(newReactionType.ReactionTypeId);
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task AddSpotReaction_WhenReactionDoesNotExistAndSpotDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        Guid invalidSpotId = Guid.NewGuid();
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);

        ReactionTypeEntity reactionType = new()
        {
            Name = "Name",
            Emoji = "Emoji",
        };
        DbContext.ReactionTypes.Add(reactionType);
        
        await DbContext.SaveChangesAsync();
        
        AddSpotReactionRequestDto requestDto = new() { ReactionTypeId = reactionType.ReactionTypeId };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(SpotsUri.SpotReaction(invalidSpotId), requestDto);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddSpotReaction_WhenReactionDoesNotExist_CreatesReactionAndReturnsNoContent()
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
        
        AddSpotReactionRequestDto requestDto = new() { ReactionTypeId = reactionType.ReactionTypeId };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(SpotsUri.SpotReaction(spot.SpotId), requestDto);

        // Assert
        SpotReactionEntity? createdSpotReaction = await DbContext.SpotReactions.AsNoTracking().FirstOrDefaultAsync(x => x.CreatorId == currentUser.UserId);
        createdSpotReaction.Should().NotBeNull();
        createdSpotReaction.ReactionTypeId.Should().Be(reactionType.ReactionTypeId);
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}