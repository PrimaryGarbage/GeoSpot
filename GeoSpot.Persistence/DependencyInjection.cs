using GeoSpot.Common.Exceptions;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeoSpot.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddGeospotDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InitializationException("Failed to find database connection string");
    
        services.AddDbContext<GeoSpotDbContext>(options => 
        options.UseNpgsql(connectionString,
            o =>
            {
                o.MapEnum<AccountType>();
                o.MapEnum<Gender>();
                o.MapEnum<SpotType>();
                o.MapEnum<Platform>();
            })
            .UseSnakeCaseNamingConvention()
        );
            
        return services;
    }
}