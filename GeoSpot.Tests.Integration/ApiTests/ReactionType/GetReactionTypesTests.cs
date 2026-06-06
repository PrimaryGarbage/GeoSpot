using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Contracts.ReactionType;
using GeoSpot.Persistence.Entities;
using static GeoSpot.Tests.Integration.ApiUriPaths;

namespace GeoSpot.Tests.Integration.ApiTests.ReactionType;

public class GetReactionTypesTests : ApiIntegrationTestsBase
{
    public GetReactionTypesTests(ApiIntegrationFixture fixture) : base(fixture)
    {}
    
    [Fact]
    public async Task GetReactionTypes_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(ReactionTypesUri.ReactionTypes);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetReactionTypes_WhenRequestIsValid_ReturnsReactionTypes()
    {
        // Arrange
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);
        
        ReactionTypeEntity reactionType1 = new()
        {
            Name = "Name1",
            Emoji = "Emoji1"
        };
        ReactionTypeEntity reactionType2 = new()
        {
            Name = "Name2",
            Emoji = "Emoji2"
        };
        DbContext.ReactionTypes.Add(reactionType1);
        DbContext.ReactionTypes.Add(reactionType2);
        
        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(ReactionTypesUri.ReactionTypes);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await responseMessage.Content.ReadFromJsonAsync<GetReactionTypesResponseDto>();
        response.Should().NotBeNull();
        response.ReactionTypes.Should().HaveCount(2);
        response.ReactionTypes.Should().ContainSingle(x => x.ReactionTypeId == reactionType1.ReactionTypeId);
        response.ReactionTypes.Should().ContainSingle(x => x.ReactionTypeId == reactionType2.ReactionTypeId);
    }
}