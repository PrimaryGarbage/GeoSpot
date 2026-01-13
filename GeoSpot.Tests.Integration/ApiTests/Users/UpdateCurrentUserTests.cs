using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GeoSpot.Common.Enums;
using GeoSpot.Contracts.User;
using GeoSpot.Persistence.Entities;
using GeoSpot.Tests.Integration.Constants;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Tests.Integration.ApiTests.Users;

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
        HttpResponseMessage responseMessage = await client.PutAsync(UriConstants.Users.UpdateCurrentUser, null);
        
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
        UserEntity userActor = await AuthorizeClientAsync(client);
        
        UserEntity userEntity = (await DbContext.Users.FindAsync(userActor.UserId))!;
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
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(UriConstants.Users.UpdateCurrentUser, requestDto);
        
        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCurrentUser_WhenUserExists_ReturnsUpdatedUser()
    {
        // Arrange
        const string updatedDisplayName = "updated_display_name";
        const string updatedAvatarUrl = "updated_avatar_url.com";
        const string updatedEmail = "updatedemail@mail.com";
        const int updatedBirthYear = 1990;
        const int updatedDetectionRadius = 50;
        const Gender updatedGender = Gender.Other;
        HttpClient client = CreateClient();
        await AuthorizeClientAsync(client);
        
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
        HttpResponseMessage responseMessage = await client.PutAsJsonAsync(UriConstants.Users.UpdateCurrentUser, requestDto);
        responseMessage.IsSuccessStatusCode.Should().BeTrue();
        UserDto? result = await responseMessage.Content.ReadFromJsonAsync<UserDto>();

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().NotBeEmpty();
        result.DisplayName.Should().Be(updatedDisplayName);
        result.AvatarUrl.Should().Be(updatedAvatarUrl);
        result.BirthYear.Should().Be(updatedBirthYear);
        result.Email.Should().Be(updatedEmail);
        result.DetectionRadius.Should().Be(updatedDetectionRadius);
        result.Gender.Should().Be(updatedGender);
    }
}