using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Contracts.User;
using GeoSpot.Persistence.Entities;
using GeoSpot.Tests.Integration.Constants;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Tests.Integration.ApiTests.Users;

public sealed class GetCurrentUserTests : ApiIntegrationTestsBase, IClassFixture<ApiIntegrationFixture>
{
    public GetCurrentUserTests(ApiIntegrationFixture fixture) : base(fixture)
    { }

    [Fact] 
    public async Task GetCurrentUser_WhenNotAuthorized_ReturnsUnauthorized()
    {
        // Arrange
        HttpClient client = CreateClient();
        
        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(UriConstants.Users.GetCurrentUser);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    
    [Fact]
    public async Task GetCurrentUser_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);
        UserEntity userEntity = (await DbContext.Users.FindAsync(userActor.UserId))!;
        DbContext.Entry(userEntity).State = EntityState.Deleted;
        await DbContext.SaveChangesAsync();
        
        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(UriConstants.Users.GetCurrentUser);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCurrentUser_WhenUserExists_ReturnsCurrentUser()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);

        // Act
        HttpResponseMessage responseMessage = await client.GetAsync(UriConstants.Users.GetCurrentUser);
        UserDto? response = await responseMessage.Content.ReadFromJsonAsync<UserDto>();

        // Assert
        response.Should().NotBeNull();
        response.UserId.Should().Be(userActor.UserId);
    }
}