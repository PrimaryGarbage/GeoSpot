using System.Text;
using GeoSpot.Common.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GeoSpot.Api.Initialization;

internal static class AuthenticationInitialization
{
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        { 
            string issuer = configuration["Jwt:Issuer"] 
                ?? throw new InitializationException("Failed to find Jwt:Issuer configuration value");
            string audience = configuration["Jwt:Audience"] 
                ?? throw new InitializationException("Failed to find Jwt:Audience configuration value");
            string signingKey = configuration["Jwt:Key"]
                ?? throw new InitializationException("Failed to find Jwt:Key configuration value");
            
            options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
            };
        });
        
        // Add authorization policies here
        services.AddAuthorization();
    }
}