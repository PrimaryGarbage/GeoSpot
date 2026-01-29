using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Common.Enums;
using GeoSpot.Contracts.User;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using static GeoSpot.Tests.Integration.Constants.UriConstants;

namespace GeoSpot.Tests.Integration.ApiTests.User;

public class UpdateCurrentUserTests : ApiIntegrationTestsBase
{
    public UpdateCurrentUserTests(ApiIntegrationFixture fixture) : base(fixture)
    { }
    
    [Fact]
    public async Task UpdateCurrentUser_WhenUnauthorized_ReturnsUnauthorized()
    {
        // Arrange
        HttpClient client = CreateClient();
        
        // Act
        HttpResponseMessage responseMessage = await client.PutAsync(UsersUri.CurrentUser, null);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateCurrentUser_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        const string updatedDisplayName = "updated_display_name";
        const string updatedAvatarUrl = "updated_avatar_url.com";
        const string updatedEmail = "updatedemail@mail.com";
        const int updatedBirthYear = 1990;
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);
        
        UserEntity userEntity = (await DbContext.Users.FindAsync(currentUser.UserId))!;
        DbContext.Entry(userEntity).State = EntityState.Deleted;
        await DbContext.SaveChangesAsync();
        
        UpdateCurrentUserRequestDto requestDto = new()
        {
            DisplayName = updatedDisplayName,
            AvatarUrl = updatedAvatarUrl,
            BirthYear = updatedBirthYear,
            Email = updatedEmail,
        };
        
        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(UsersUri.CurrentUser, requestDto);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCurrentUser_WhenUserExists_ReturnsOk()
    {
        // Arrange
        const string updatedDisplayName = "updated_display_name";
        const string updatedAvatarUrl = "updated_avatar_url.com";
        const string updatedEmail = "updatedemail@mail.com";
        const int updatedBirthYear = 1990;
        const int updatedDetectionRadius = 50;
        const Gender updatedGender = Gender.Other;
        HttpClient client = CreateClient();
        UserEntity currentUser = await AuthorizeClientAsync(client);
        
        UpdateCurrentUserRequestDto requestDto = new()
        {
            DisplayName = updatedDisplayName,
            AvatarUrl = updatedAvatarUrl,
            BirthYear = updatedBirthYear,
            Email = updatedEmail,
            DetectionRadius = updatedDetectionRadius,
            Gender = updatedGender
        };

        // Act
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(UsersUri.CurrentUser, requestDto);
        responseMessage.IsSuccessStatusCode.Should().BeTrue();

        // Assert
        UserEntity? updatedUser = await DbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == currentUser.UserId);
        updatedUser.Should().NotBeNull();
        updatedUser.UserId.Should().NotBeEmpty();
        updatedUser.DisplayName.Should().Be(updatedDisplayName);
        updatedUser.AvatarUrl.Should().Be(updatedAvatarUrl);
        updatedUser.BirthYear.Should().Be(updatedBirthYear);
        updatedUser.Email.Should().Be(updatedEmail);
        updatedUser.DetectionRadius.Should().Be(updatedDetectionRadius);
        updatedUser.Gender.Should().Be(updatedGender);
    }
}