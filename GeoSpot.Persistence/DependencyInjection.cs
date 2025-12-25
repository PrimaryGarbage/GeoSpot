using GeoSpot.Common.Exceptions;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeoSpot.Persistence;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddGeospotDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("SqlDatabase")
            ?? throw new InitializationException("Failed to find database connection string");
    
        services.AddDbContext<GeoSpotDbContext>(options => 
        options.UseNpgsql(connectionString,
            o =>
            {
                o.UseNetTopologySuite(geographyAsDefault: true);
                o.MapEnum<AccountType>();
                o.MapEnum<Gender>();
                o.MapEnum<SpotType>();
                o.MapEnum<Platform>();
            })
            .UseSnakeCaseNamingConvention()
        );
            
        return services;
    }
    
    public static void PrepareDatabase(this IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GeoSpotDbContext>();
        db.Database.MigrateAsync();
    }
}