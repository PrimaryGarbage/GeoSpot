using System.Net.Http.Headers;
using System.Net.Http.Json;
using GeoSpot.Api.Constants;
using GeoSpot.Application.Mappers.User;
using GeoSpot.Application.Services;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Auth;
using GeoSpot.Persistence;
using GeoSpot.Persistence.Repositories.Models.User;
using GeoSpot.Tests.Integration.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Tests.Integration.ApiTests;

[ExcludeFromCodeCoverage]
[Collection(CollectionConstants.ApiIntegrationCollectionName)]
public abstract class ApiIntegrationTestsBase : IAsyncLifetime
{
    
    private protected GeoSpotDbContext DbContext => _fixture.DbContext;
    
    private readonly ApiIntegrationFixture _fixture;
    
    private protected ApiIntegrationTestsBase(ApiIntegrationFixture fixture)
    {
        _fixture = fixture;
    }

    protected HttpClient CreateClient() => _fixture.CreateHttpClient();
    
    protected async Task<UserModel> AuthorizeClientAsync(HttpClient client)
    {
        const string phoneNumber = "+777777";
        MockVerificationCodeGenerator codeGenerator = new();
        
        await client.PostAsJsonAsync(UriConstants.Auth.SendVerificationCode,
            new SendVerificationCodeRequestDto(phoneNumber));
        
        HttpResponseMessage responseMessage = await client.PostAsJsonAsync(UriConstants.Auth.VerifyVerificationCode, 
            new VerifyVerificationCodeRequestDto(phoneNumber, codeGenerator.GenerateCode(6)));
        
        VerifyVerificationCodeResponseDto response = await responseMessage.Content.ReadFromJsonAsync<VerifyVerificationCodeResponseDto>()
            ?? throw new InternalProblemException("Failed to authorize http client");
        
        if (response.CreatedUser is null)
            throw new InternalProblemException("Failed to authorize http client: created user is null");
        
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, response.Tokens.AccessToken);
        
        return response.CreatedUser.MapToModel();
    }
    
    protected HttpClient SetApiVersion(HttpClient client, string version)
    {
        client.DefaultRequestHeaders.Add(HeaderConstants.VersioningHeaderName, version);
        
        return client;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return ResetDatabaseAsync();
    }

    private async Task ResetDatabaseAsync()
    {
        List<string> tables = DbContext.Model.GetEntityTypes()
            .Where(t => t.GetTableName() is not null)
            .Select(t =>
            {
                string schema = t.GetSchema() ?? "public";
                string? name = t.GetTableName();
                return $"\"{schema}\".\"{name}\"";
            })
            .Distinct()
            .ToList();

        if (tables.Count == 0) return;

        string sql = $"TRUNCATE TABLE {string.Join(", ", tables)} RESTART IDENTITY CASCADE;";
        await DbContext.Database.ExecuteSqlRawAsync(sql);
    }
}