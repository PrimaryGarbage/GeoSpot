using System.Net;
using FluentAssertions;
using GeoSpot.Persistence.Entities;
using GeoSpot.Tests.Integration.Constants;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Tests.Integration.ApiTests.Auth;

public class LogoutUserTests : ApiIntegrationTestsBase
{

    public LogoutUserTests(ApiIntegrationFixture fixture) : base(fixture) {}
    
    [Fact]
    public async Task LogoutUser_WhenNoJwtToken_ReturnsUnauthorized()
    {
        // Arrange
        HttpClient client = CreateClient();
        
        // Act
        HttpResponseMessage responseMessage = await client.PostAsync(UriConstants.Auth.LogoutUser, null);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LogoutUser_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity userActor = await AuthorizeClientAsync(client);
        
        DbContext.Entry(DbContext.Users.First(x => x.UserId ==userActor.UserId)).State = EntityState.Deleted;
        await DbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage responseMessage = await client.PostAsync(UriConstants.Auth.LogoutUser, null);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task LogoutUser_WhenAccessTokenIsValidAndUserExists_ReturnsOK()
    {
        // Arrange
        HttpClient client = CreateClient();
        UserEntity _ = await AuthorizeClientAsync(client);

        // Act
        HttpResponseMessage responseMessage = await client.PostAsync(UriConstants.Auth.LogoutUser, null);

        // Assert
        responseMessage.IsSuccessStatusCode.Should().BeTrue();
    }
}